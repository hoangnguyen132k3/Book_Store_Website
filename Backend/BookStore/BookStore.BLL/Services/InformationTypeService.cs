using BookStore.BLL.Services.Base;
using BookStore.BLL.Services.ViewModels;
using BookStore.DAL.Infrastructure;
using BookStore.DAL.Models;
using Microsoft.Data.SqlClient.DataClassification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InformationType = BookStore.DAL.Models.InformationType;

namespace BookStore.BLL.Services
{
    public interface IInformationTypeService : IBaseService<InformationType>
    {
        Task<InformationTypeVm> AddInformationTypeAsync(InputInformationTypeVm informationTypeVm);
        Task<InformationTypeVm> UpdateInformationTypeAsync(int id, InputInformationTypeVm informationTypeVm);

        Task<InformationTypeVm> DeleteInformationTypeAsync(int id);

        Task<InformationTypeVm?> GetByInformationTypeIdAsync(int id);

        Task<IEnumerable<InformationTypeVm>> GetAllInformationTypeAsync();
    }
    public class InformationTypeService : BaseService<InformationType>, IInformationTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InformationTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<InformationTypeVm>> GetAllInformationTypeAsync()
        {
            var informationTypes = await GetAllAsync();

            var InformationTypeViewModels = informationTypes.Select(informationType => new InformationTypeVm
            {
                InformationTypeId = informationType.InformationTypeId,
                Code = informationType.Code,
                CreatedAt = informationType.CreatedAt,
                UpdatedAt = informationType.UpdatedAt
            });

            return InformationTypeViewModels;
        }

        public async Task<InformationTypeVm?> GetByInformationTypeIdAsync(int id)
        {
            var informationType = await GetByIdAsync(id);
            if (informationType == null) return null;
            var informationTypeVm = new InformationTypeVm
            {
                InformationTypeId = informationType.InformationTypeId,
                Code = informationType.Code,
                CreatedAt = informationType.CreatedAt,
                UpdatedAt = informationType.UpdatedAt
            };

            return informationTypeVm;
        }
        public async Task<InformationTypeVm> AddInformationTypeAsync(InputInformationTypeVm informationTypeVm)
        {
            ValidateModelPropertiesWithAttribute(informationTypeVm);

            var findInformationType = await _unitOfWork.GenericRepository<InformationType>().GetAsync(b =>
                b.Code == informationTypeVm.Code
            );

            if (findInformationType != null)
            {
                throw new ExceptionBusinessLogic("Tên thông tin đã được sử dụng.");
            }

            var informationType = new InformationType
            {
                Code = informationTypeVm.Code
            };

            var result = await AddAsync(informationType);
            if (result <= 0)
            {
                throw new ArgumentException("Không thể thêm thông tin.");
            }

            return new InformationTypeVm
            {
                InformationTypeId = informationType.InformationTypeId,
                Code = informationType.Code
            };
        }

        public async Task<InformationTypeVm> UpdateInformationTypeAsync(int id, InputInformationTypeVm informationTypeVm)
        {
            ValidateModelPropertiesWithAttribute(informationTypeVm);

            var informationType = await _unitOfWork.GenericRepository<InformationType>().GetByIdAsync(id);
            if (informationType == null)
            {
                throw new ArgumentException("Thông tin không tìm thấy.");
            }

            var findInformationType = await _unitOfWork.GenericRepository<InformationType>().GetAsync(b =>
                b.InformationTypeId != id &&
                b.Code == informationTypeVm.Code
            );

            if (findInformationType != null)
            {
                throw new ExceptionBusinessLogic("Tên thông tin đã được sử dụng.");
            }

            informationType.Code = informationTypeVm.Code;
            informationType.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.GenericRepository<InformationType>().ModifyAsync(informationType);
            if (result <= 0)
            {
                throw new ArgumentException("Không thể cập nhật thông tin.");
            }

            return new InformationTypeVm
            {
                InformationTypeId = informationType.InformationTypeId,
                Code = informationType.Code,
                CreatedAt = informationType.CreatedAt
            };
        }

        public async Task<InformationTypeVm> DeleteInformationTypeAsync(int id)
        {
            var informationType = await _unitOfWork.GenericRepository<InformationType>().GetByIdAsync(id);

            if (informationType == null) return null;

            _unitOfWork.GenericRepository<InformationType>().Delete(id);

            if (_unitOfWork.SaveChanges() > 0)
            {
                return new InformationTypeVm
                {
                    InformationTypeId = informationType.InformationTypeId,
                    Code = informationType.Code
                };
            }

            throw new ArgumentException("Không thể xóa thông tin");

        }

    }
}
