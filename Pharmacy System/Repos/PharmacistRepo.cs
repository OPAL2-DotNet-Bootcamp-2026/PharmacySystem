using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistRepo
    {
        private PharmacyContext context;

        public PharmacistRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public List<Pharmacist> GetAllPharmacist()
        {
            return context.pharmacists.ToList();
        }

        public Pharmacist GetPharmacistById(int id)
        {
            return context.pharmacists.FirstOrDefault(p => p.PharmacistID == id);
        }

        public Pharmacist GetPharmacistByEmail(string email)
        {
            return context.pharmacists.FirstOrDefault(e => e.Email == email);
        }

        public bool EmailExists(string email)
        {
            return context.pharmacists.Any(e => e.Email == email);
        }

        public void Add(Pharmacist pharmacists)
        {
            context.pharmacists.Add(pharmacists);
            context.SaveChanges();
        }

        public void PharmacistUpdate()
        {
            context.SaveChanges();
        }

        public void PharmacistDelete(Pharmacist pharmacists)
        {
            context.pharmacists.Remove(pharmacists);
            context.SaveChanges();
        }
    }
}
