using API.Context;
using Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace API.Services.Implement
{
    public class CustomerInformationSvc : ILookupSvc<int, CustomerInformation>, IAddable<CustomerInformation>, IReadable<CustomerInformation>, IDeletable<int, CustomerInformation>, IEditable<CustomerInformation>
    {
        private readonly FastFoodDBContext _dbContext;
        public CustomerInformationSvc(FastFoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int NewId()
        {
            int id = _dbContext.customerInformations.Count() + 1;
            int count = 0;
            while(_dbContext.customerInformations.Any(x => x.CInforId == id))
            {
                count++;
                id += count;
            }
            return id;
        }

        /// <summary>
        /// Thêm một thông tin khách hàng mới
        /// </summary>
        /// <remarks>
        /// Mẫu:
        /// {
        ///     customerName: 'Trần Văn B',
        ///     phoneNumber: '0394857621',
        ///     address: 'Công viên phần mềm Quang Trung',
        ///     customerEmail: '...' (tài khoản khách hàng đã tạo)
        ///  }
        /// </remarks>
        /// <returns></returns>
        public async Task<CustomerInformation> AddNewData(CustomerInformation entity)
        {
            entity.CInforId = NewId();
            if(_dbContext.customers.Any(x => x.Email != entity.CustomerEmail))
            {
                return null;
            }
            await _dbContext.customerInformations.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Xóa một thông tin khách hàng 
        /// </summary>
        /// <param name="key">cInforId</param>
        /// <returns></returns>
        public async Task<string> DeleteData(int key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == key).FirstOrDefaultAsync();
            if(find == default)
            {
                return "Không tìm thấy";
            }
            _dbContext.customerInformations.Remove(find);
            await _dbContext.SaveChangesAsync();
            return $"Xóa {key} thành công !";
        }

        /// <summary>
        /// Sửa một thông tin khách hàng theo id
        /// </summary>
        /// <returns>Thông tin khách hàng đã sửa</returns>
        public async Task<CustomerInformation> EditData(CustomerInformation entity)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == entity.CInforId).FirstOrDefaultAsync();
            if(find == default)
            {
                return null;
            }
            find.PhoneNumber = entity.PhoneNumber;
            find.CustomerName = entity.CustomerName;
            find.Address = entity.Address;
            await _dbContext.SaveChangesAsync();
            return find;
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo id
        /// </summary>
        /// <param name="key">cInforId</param>
        /// <returns>Thông tin khách hàng</returns>
        public async Task<CustomerInformation> GetDataByKey(int key)
        {
            var find = await _dbContext.customerInformations.Where(x => x.CInforId == key).FirstOrDefaultAsync();
            if (find == default)
            {
                return null;
            }
            return find;
        }

        /// <summary>
        /// Lấy danh sách thông tin khách hàng
        /// </summary>
        /// <returns>Danh sách thông tin khách hàng</returns>
        public async Task<IEnumerable<CustomerInformation>> ReadDatas()
        {
            return await _dbContext.customerInformations.ToListAsync();
        }
    }
}
