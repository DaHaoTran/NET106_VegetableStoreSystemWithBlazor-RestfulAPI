﻿using API.Services.Interfaces;
using DTO;
using Microsoft.EntityFrameworkCore;
using Models;

namespace API.Services.Implement
{
    public class GuestSvc : IAddable<Guest>, IDeletable<int, Guest>, ILookupSvc<int, Guest>, IReadable<Guest>, ILookupSvc<string, Guest>, ILookupMoreSvc<string, Guest>
    {
        private readonly FastFoodDBContext _dbContext;
        public GuestSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int NewId()
        {
            int id = _dbContext.guests.Count() + 1;
            int count = 0;
            while (_dbContext.guests.Any(x => x.GuesId == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        public async Task<Guest> AddNewData(Guest entity)
        {
            entity.GuesId = NewId();
            await _dbContext.guests.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteData(int key)
        {
            var find = await _dbContext.guests.Where(x => x.GuesId == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.guests.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        public async Task<Guest> GetDataByKey(int key)
        {
            var data = await _dbContext.guests.Where(x => x.GuesId == key).FirstOrDefaultAsync();
            if (data == default)
            {
                return null;
            }
            return data;
        }

        public async Task<IEnumerable<Guest>> ReadDatas()
        {
            return await _dbContext.guests.ToListAsync();
        }

        public async Task<Guest> GetDataByKey(string key)
        {
            var find = await _dbContext.guests.Where(x => x.PhoneNumber == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            return find;
        }

        public async Task<IEnumerable<Guest>> GetListByKey(string key)
        {
            var find = await _dbContext.guests.Where(x => x.GuestName.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
            if(find.Count == 0)
            {
                find = await _dbContext.guests.Where(x => x.Address.Contains(key, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                if(find.Count == 0)
                {
                    return null;
                }
            }
            return find;
        }
    }
}
