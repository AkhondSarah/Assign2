using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Lab4N.Data;
using Lab4N.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab4N.Models.ViewModels;

namespace Lab4N.Controllers
{
    public class AdvertisementsController : Controller
    {

        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "advertisements";

        public AdvertisementsController(MarketDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index(string id)
        {
            var brokerage = _context.Brokerages.Where(m => m.Id == id).FirstOrDefault();
            var adsView = new AdsViewModel();
            adsView.Brokerage = brokerage;
            var adsBrokerage = _context.AdvertisementBrokerage.Where(m => m.BrokerageId == id).Include(m => m.Advertisement);

            adsView.Advertisements = adsBrokerage.Select(m => m.Advertisement);

            return View(adsView);
        }

        public IActionResult Create(string id)
        {
            return View(_context.Brokerages.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string BrokerageId, IFormFile file)
        {

            BlobContainerClient containerClient;
            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(file.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var image = new Advertisement();
                image.Url = blockBlob.Uri.AbsoluteUri;
                image.FileName = file.FileName;
                _context.Advertisements.Add(image);
                _context.SaveChanges();

                var adsBrokerage = new AdsBrokerage();
                adsBrokerage.AdvertisementID = image.Id;
                adsBrokerage.BrokerageId = BrokerageId;

                _context.AdvertisementBrokerage.Add(adsBrokerage);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                View("Error");
            }

            return RedirectToAction("Index", new { id = BrokerageId });
        }

    

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Advertisements.FindAsync(id);
           string brokerageID = _context.AdvertisementBrokerage.Where(m => m.AdvertisementID == id).First().BrokerageId;


            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                // Get the blob that holds the data
                var blockBlob = containerClient.GetBlobClient(image.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                _context.Advertisements.Remove(image);
                await _context.SaveChangesAsync();

            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            return RedirectToAction("Index", new { id = brokerageID });
        }
    }
}
