﻿using Inventory.Contracts;
using Inventory.Data;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class BrandRepository : IBrandRepository
    {
        private readonly InventoryContext _ctx;

        public BrandRepository(InventoryContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Brand> CreateBrand(Brand Brand)
        {
            if (Brand == null)
            {
                throw new ArgumentNullException(nameof(Brand));
            }
            if (await BrandDoesExist(Brand.Name))
            {
                return await _ctx.Brands.FirstOrDefaultAsync(b => b.Name == Brand.Name);
            }

            
            
            Brand.CreatedAt = DateTime.UtcNow;
            Brand.LastUpdated = DateTime.UtcNow;

            await _ctx.Brands!.AddAsync(Brand);

            if (await Save())
            {
                return await _ctx.Brands.FirstOrDefaultAsync(b => b.Id == Brand.Id);
            }

            return null;
        }

        public async Task<bool> DeleteBrand(Guid Id)
        {
            if (!await BrandDoesExist(Id))
            {
                return await Save();
            }
            var deletedBrand = await GetBrandById(Id);

            _ctx.Brands?.Remove(deletedBrand);

            return await Save();
        }

        public async Task<Brand> GetBrandById(Guid Id)
        {
            if (Id == null)
            {
                throw new ArgumentException(nameof(Id));
            }

            return await _ctx.Brands.FindAsync(Id);
        }

        public async Task<Brand> GetBrandByName(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException(nameof(Name));
            }

            Brand Brand = await _ctx.Brands.FirstOrDefaultAsync(b => b.Name == Name);
            return Brand;
        }

        public async Task<IEnumerable<Brand>> GetBrands()
        {
            return _ctx.Brands.Where(b => b.Name != null);
        }

        public async Task<bool> Save()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<Brand> UpdateBrand(Brand Brand)
        {
            if (Brand == null)
            {
                throw new ArgumentNullException(nameof(Brand));
            }
            if (!await BrandDoesExist(Brand.Id))
            {
                throw new ArgumentException(nameof(Brand));
            }
        
            
            Brand.LastUpdated = DateTime.UtcNow;
            _ctx.Brands!.Update(Brand);
            await Save();
            return Brand;
        }

        public async Task<bool> BrandDoesExist(string BrandName)
        {
            if (string.IsNullOrEmpty(BrandName))
            {
                throw new ArgumentException(nameof(BrandName));
            }
            var existing = await GetBrandByName(BrandName);

            return existing != null;
        }

        public async Task<bool> BrandDoesExist(Guid BrandId)
        {
            var existing = await GetBrandById(BrandId);

            return existing != null;
        }

    }
}
