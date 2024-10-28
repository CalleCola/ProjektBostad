namespace ProjektBostad.Models
{
    public class BostadsDetalj
    {
        public int Anlagg_Id { get; set; }
        public int Kostnad { get; set; }
        public int Kvm { get; set; }
        public int Typ_Id { get; set; }
        public int Maklar_Id { get; set; }
        public string? Adress { get; set; }
        public string? Bostadsnamn { get; set; }
        public string? Postkod { get; set; }
        public string? Stad { get; set; }
        public string? Bild { get; set; }

        public string Test { get; set; }
    }
}
