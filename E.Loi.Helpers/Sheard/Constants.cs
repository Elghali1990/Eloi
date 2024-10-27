namespace E.Loi.Helpers.Sheard;

public class Constants
{
    public static string CategoryLaw = "PLF";
    public static string CGI = "CGI";
    public static string Douane = "Douane";
    public static Dictionary<string, string> AmendmentIntents = new Dictionary<string, string>()
    {
        {"MODIFICATION","MODIFICATION" },
        {"DELETION","DELETION" },
        {"ADDITION","ADDITION" }
    };

    public static Dictionary<string, string> AmendmentTypes = new Dictionary<string, string>()
    {
        {"SINGLE","SINGLE" },
        {"CONSENSUS","CONSENSUS" },
        {"HARMONIZATION","HARMONIZATION" }
    };



    public static Dictionary<string, string> NodeTypes = new Dictionary<string, string>()
    {
        {"rubrique","الباب" },
        {"article","المادة" },
        {"Paragraphe","الفقرة" },
        {"branche","الفرع" },
        {"ProjetLoi","مشروع قانون" },
        {"chapitre","الفصل"},
        {"partie","الجزء"},
        {"section","القسـم"}
    };
    public static int pageSize = 10;
    public static string Paragraphe = "الفقرة";
    public static string Clause = "البند";
    public static string clause = "بند";
    public static string Article = "المادة";
   // public static string article = "مادة";
    public static string articleMitier = "مادة وظيفية";
    public static string Title = "عنوان";

    public static string Open = "مفتوح";
    public static string Closed = "مغلق";
    public static string Public = "عام";
    public static string Submited = "محال";

    public static string MessageTeamRequired = " المرجو إختيار صاحب التعدبل";
    public static string SuccessOperation = "تمت العملية بنجاح.";
    public static string SuccessDeleteOperation = "تمت عملية الحذف بنجاح.";
    public static string MessageIntentRequired = "المرجو تحديد طبيعة التعديل";
    public static string MessageNumberRequired = "المرجو إدخال رقم التعديل .";
    public static string MessageOriginalTextRequired = "المرجو إدخال النص الاصلي .";
    public static string MessageSelectAmendments = "المرجو إختيار التعديلات التي سيبنى عليها التعديل الجديد.";
    public static string MessageSelectNodeType = "المرجو إختيار نوع العقدة.";
    public static string MessageNumberNode = "المرجو إدخال عدد .";
    public static string MessageAmendmentContentRequired = "المرجو إدخال نص التعديل .";
    public static string MessageSuccessAddAmendment = "تمت إضافة تعديل بنجاح";
    public static string MessageSuccessUpdateAmendment = "تم تحديث التعديل بنجاح";
    public static string MessageSuccessDeleteAmandment = "تم حذف التعديل بنجاح";
    public static string MessageSuccessCloseAmendment = "تم إغلاق التعديلات بنجاح.";
    public static string MessageSuccessPublishAmendment = "تم نشر التعديلات بنجاح.";
    public static string MessageSuccessOpenAmendment = "تم فتح التعديلات بنجاح.";
    public static string MessageSuccessSubmitAmendment = "تمت إحالة التعديلات على اللجنة بنجاح.";
    public static string MessageError = "خطأ في النظام المرجو إتصال بمدير النظام.";
    public static string MessageUploadAmendments = "تم التشبت بالتعديلات بنجاح.";
    public static string MessageErrorSetAmendmentStatu = "المرجو إختيار التعديلات المغلقة أو المحالة .";
    public static string MessageLoadLawContent = "تم تحميل محتوى بنجاح.";
    public static string TextAlreadyExists = "هذا النص موجود مسبقا.";
    public static string ShowCanvas = "ShowCanvas";
    public static string HideModal = "HideModal";
    public static string ShowModal = "ShowModal";
    public static string HideCanvas = "HideCanvas";
    public static string ADDITION = "ADDITION";
    public static string DELETION = "DELETION";
    public static string MODIFICATION = "MODIFICATION";
    public static string MESSAGELOGINFIELD = "إسم المستخدم أو كلمة المرور غير صحيحة.";
    public static string SendTeamsMessage = "تمت عملية الإرسال بنجاح .";
    public static string NumberAmendmentExiste = "رقم هذا التعديل موجود المرجو إدخال رقم أخر .";
    public static string GetVoteResult(string result) => result switch
    {
        "VALID" => "مقبول",
        "REJECT" => "غير مقبول",
        "PARTIAL" => "مقبول جزءيا",
        "UNANIMOUS" => "بالإجماع",
        "CONSENSUS" => "توافقي",
        "SUPPRESSED" => "مسحوب",
        _ => string.Empty
    };

    public static int GetPhaseOrder(string phase) => phase switch
    {
        "1.1" => 2,
        "1.2" => 3,
        "2.1" => 6,
        "2.2" => 7,
        _ => 0
    };



    public static string GetPhaseByOrder(int order) => order switch
    {
        2 => "1.1",
        3 => "1.2",
        6 => "2.1",
        7 => "2.2",
        _ => string.Empty
    };

    public static string GetAmendmentStatu(string statu) => statu switch
    {
        "EDITABLE" => "مفتوح",
        "FINAL" => "مغلق",
        "SUBMITTED" => "محال",
        "PUBLIC" => "عام",
        _ => string.Empty
    };

    public static string GetAmendmentIntent(string AmendmentIntent) => AmendmentIntent switch
    {
        "MODIFICATION" => "تغيير أو تتميم",
        "DELETION" => "نسخ",
        _ => "تتميم"
    };


    public static Dictionary<string, string> nodeTypes = new()
    {
        {"Law","قانون" },
        {"Branch","الفرع" },
        {"ArticleMitier","مادة وظيفية" },
        {"Clause","البند" },
        {"Anexxe","الملحق" },
        {"Subject","المادة" },
        {"Title","عنوان" },
        {"Door","الباب" },
        {"Paragraph","الفقرة" },
        {"Section","الجزء" },
        {"Chapitre","فصل" },
        {"Book","الكتــاب" },
        {"Division","القسـم" },
        {"DivisionChild","القسم الفرعي" },
        {"ListTarifaireDouanes","جدول التعريفة الجمركية" }
    };

    public static string GetNodelabel(string NodeTypeLabel, int Number, string NodeLabel, int Bis)
    {
        string repeated = string.Empty;
        string label = string.Empty;
        if (nodeTypes["Section"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Door"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Paragraph"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Branch"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Subject"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel}  {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Clause"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Title"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["Anexxe"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["ListTarifaireDouanes"].Trim() == NodeTypeLabel.Trim())
        {
            label = $"{NodeTypeLabel} {Number}  :  {NodeLabel}";
        }
        else if (nodeTypes["ArticleMitier"].Trim() == NodeTypeLabel.Trim())
        {
            repeated = Bis > 0 ? "مكرر " + Bis + ""  :   string.Empty;
            label = $"{NodeTypeLabel} {Number}  {repeated} :  {NodeLabel}";
        }
        else if (nodeTypes["Book"].Trim() == NodeTypeLabel.Trim())
        {
            repeated = Bis > 0 ? "مكرر " + Bis + ""  :   string.Empty;
            label = $"{NodeTypeLabel} {Number}  {repeated} :  {NodeLabel}";
        }
        else if (nodeTypes["Division"].Trim() == NodeTypeLabel.Trim())
        {
            repeated = Bis > 0 ? "مكرر " + Bis + ""  :   string.Empty;
            label = $"{NodeTypeLabel} {Number}  {repeated} :  {NodeLabel}";
        }   
        else if (nodeTypes["DivisionChild"].Trim() == NodeTypeLabel.Trim())
        {
            repeated = Bis > 0 ? "مكرر " + Bis + ""  :   string.Empty;
            label = $"{NodeTypeLabel} {Number}  {repeated} :  {NodeLabel}";
        }
        else if (nodeTypes["Chapitre"].Trim() == NodeTypeLabel.Trim())
        {
            repeated = Bis > 0 ? "مكرر " + Bis + ""  :   string.Empty;
            label = $"{NodeTypeLabel} {Number} {repeated} :  {NodeLabel}";
        }
        
        else
        {
            label = NodeLabel;
        }
        if (label.Trim().EndsWith(" :  "))
        {
            label = label.Replace(" :  ", string.Empty);
        }
        return label.Trim();
    }

    public static int getPageSize(int rowCount)
    {
        int pageSize = 0;
        string result = (rowCount / 10.0).ToString();
        if (result.Contains(","))
        {
            var nbr = result.Split(',');
            pageSize = int.Parse(nbr[0]) + 1;
        }
        else
        {
            pageSize = rowCount / 10;
        }
        return pageSize;
    }

    public static List<int> FillListOfPage(int count)
    {
        List<int> list = new List<int>();
        for (int i = 1; i <= count; i++)
            list.Add(i);
        return list;
    }



}
