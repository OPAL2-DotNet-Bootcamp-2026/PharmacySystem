using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacyRepo
    {
        private PharmacyContext context;

        public PharmacyRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public List<Pharmacy> GetAllPharmacy()
        {
            return context.pharmacies.ToList();
        }

        public Pharmacy GetPharmacyById(int id)
        {
            return context.pharmacies.FirstOrDefault(p => p.PharmacyID == id);
        }

        public void Add(Pharmacy pharmacy)
        {
            context.pharmacies.Add(pharmacy);
            context.SaveChanges();
        }

        public void Update()
        {
            context.SaveChanges();
        }

        public void Delete(Pharmacy pharmacy)
        {
            context.pharmacies.Remove(pharmacy);
            context.SaveChanges();
        }
    }
}
