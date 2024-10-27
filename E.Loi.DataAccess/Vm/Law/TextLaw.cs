namespace E.Loi.DataAccess.Vm.Law;
public class TextLaw
{
    public Guid IdNoeud { get; set; }

    public string TypeNoeud { get; set; } = default!;

    public Guid? IdNoeudParent { get; set; }

    public int? NumeroArticle { get; set; }

    public string TitreAr { get; set; } = default!;

    public string TitreFr { get; set; } = default!;

    public string ContenuAr { get; set; } = default!;

    public string ContenuFr { get; set; } = default!;

    public int OrdreNoeud { get; set; }

    public int SousOrdreNoeud { get; set; }

    public bool AvecContenu { get; set; }
    public int OrdreGenerale { get; set; }

}