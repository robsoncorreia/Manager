using LiteDB;
using QRCodeGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRCodeGenerator.Repository
{
    public interface ILocalDBRepository
    {
        object FindOne<Tmodel>(Tmodel model);

        IEnumerable<QRCodeModel> GetAll();

        object Upsert<Tmodel>(Tmodel model);

        int ClearAll();
    }

    public class LocalDBRepository : ILocalDBRepository
    {
        private readonly string connectionString = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\0002.db";

        public object FindOne<Tmodel>(Tmodel model)
        {
            if (model is Email temp)
            {
                using LiteDatabase db = new LiteDatabase(connectionString);

                ILiteCollection<Email> emails = db.GetCollection<Email>("Email");

                return emails.FindOne(x => x.EmailId != 0);
            }

            return null;
        }

        public object Upsert<Tmodel>(Tmodel model)
        {
            if (model is Email email)
            {
                using LiteDatabase db = new LiteDatabase(connectionString);

                ILiteCollection<Email> emails = db.GetCollection<Email>("Email");

                return emails.Upsert(email);
            }

            if (model is QRCodeModel qrCode)
            {
                using LiteDatabase db = new LiteDatabase(connectionString);

                ILiteCollection<QRCodeModel> qRCodes = db.GetCollection<QRCodeModel>("QRCode");

                return qRCodes.Upsert(qrCode);
            }

            return null;
        }

        public IEnumerable<QRCodeModel> GetAll()
        {
            using LiteDatabase db = new LiteDatabase(connectionString);

            return db.GetCollection<QRCodeModel>("QRCode").FindAll().ToList();
        }

        public int ClearAll()
        {
            using LiteDatabase db = new LiteDatabase(connectionString);

            return db.GetCollection<QRCodeModel>("QRCode").DeleteAll();
        }
    }
}