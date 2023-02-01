using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain
{
    public class BaseTableDefault
    {
        public DateTime? CreatedOnDate { get; set; } = DateTime.Now; 
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
    }
    public class BaseTable<T> where T : BaseTable<T>
    {
        #region Init Create/Update

        //public T InitCreate()
        //{
        //    return InitCreate(AppConstants.HO_APP, UserConstants.AdministratorUserId);
        //}

        //public T InitCreate(Guid? application, Guid? userId)
        //{
        //    return InitCreate(application ?? AppConstants.HO_APP, userId ?? UserConstants.AdministratorUserId);
        //}

        //public T InitCreate(Guid? application, Guid userId)
        //{
        //    return InitCreate(application ?? AppConstants.HO_APP, userId);
        //}

        //public T InitCreate(Guid application, Guid? userId)
        //{
        //    return InitCreate(application, userId ?? UserConstants.AdministratorUserId);
        //}

        public T InitCreate(Guid userId)
        {
            CreatedByUserId = userId;
            LastModifiedByUserId = userId;
            CreatedOnDate = DateTime.Now;
            LastModifiedOnDate = DateTime.Now;
            return (T)this;
        }

        //public T InitUpdate()
        //{
        //    return InitUpdate(AppConstants.HO_APP, UserConstants.AdministratorUserId);
        //}

        //public T InitUpdate(Guid? application, Guid? userId)
        //{
        //    return InitUpdate(application ?? AppConstants.HO_APP, userId ?? UserConstants.AdministratorUserId);
        //}

        //public T InitUpdate(Guid? application, Guid userId)
        //{
        //    return InitUpdate(application ?? AppConstants.HO_APP, userId);
        //}

        //public T InitUpdate(Guid application, Guid? userId)
        //{
        //    return InitUpdate(application, userId ?? UserConstants.AdministratorUserId);
        //}

        public T InitUpdate( Guid userId)
        {
            LastModifiedByUserId = userId;
            LastModifiedOnDate = DateTime.Now;
            return (T)this;
        }

        #endregion Init Create/Update

        public Guid CreatedByUserId { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public DateTime LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
    }
}
