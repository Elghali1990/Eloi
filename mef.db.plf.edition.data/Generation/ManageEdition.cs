using Aspose.Words;
using Aspose.Words.Fields;
using E.Loi.DataAccess.Vm.Law;
using E.Loi.DataAccess.Vm.Node;
using E.Loi.DataAccess.Vm.Phase;
using E.Loi.Edition.Generation.Helpers;
using System.Text.RegularExpressions;

namespace mef.db.plf.edition.generation.Generation
{
    public static class ManageEdition
    {
        public static string getTitleParentArticle(NodeTitleVM node, string type, string lang)
        {
            string titleParent = string.Empty;

            if (lang == "ar")
            {
                switch (node.number)
                {
                    case 1:
                        {
                            titleParent = type + " : الأول " + node.Label;
                            break;
                        }
                    case 2:
                        {
                            titleParent = type + " : الثاني " + node.Label;
                            break;
                        }
                    case 3:
                        {
                            titleParent = type + " : الثالث " + node.Label;
                            break;
                        }
                    case 4:
                        {
                            titleParent = type + " : الرابع " + node.Label;
                            break;
                        }
                    case 5:
                        {
                            titleParent = type + " : الخامس " + node.Label;
                            break;
                        }
                    case 6:
                        {
                            titleParent = type + " : السادس " + node.Label;
                            break;
                        }
                    case 7:
                        {
                            titleParent = type + " : السابع " + node.Label;
                            break;
                        }
                    case 8:
                        {
                            titleParent = type + " : الثامن " + node.Label;
                            break;
                        }
                    case 9:
                        {
                            titleParent = type + " : التاسع " + node.Label;
                            break;
                        }
                    case 10:
                        {
                            titleParent = type + " : العاشر " + node.Label;
                            break;
                        }
                    default:
                        {
                            titleParent = type + (node.number) + "" + node.Label;
                            break;
                        }
                }
            }
            else
            {
                switch (node.number)
                {
                    case 1:
                        {
                            titleParent = "Première " + type + " : " + node.Label;
                            break;
                        }
                    case 2:
                        {
                            titleParent = "Deuxième " + type + " : " + node.Label;
                            break;
                        }
                    case 3:
                        {
                            titleParent = "Troisième " + type + " : " + node.Label;
                            break;
                        }
                    case 4:
                        {
                            titleParent = "Quatrième " + type + " : " + node.Label;
                            break;
                        }
                    case 5:
                        {
                            titleParent = "Cinquième " + type + " : " + node.Label;
                            break;
                        }
                    case 6:
                        {
                            titleParent = "Sixième " + type + " : " + node.Label;
                            break;
                        }
                    case 7:
                        {
                            titleParent = "Septième " + type + " : " + node.Label;
                            break;
                        }
                    case 8:
                        {
                            titleParent = "Huitième " + type + " : " + node.Label;
                            break;
                        }
                    case 9:
                        {
                            titleParent = "Neuvième " + type + " : " + node.Label;
                            break;
                        }
                    case 10:
                        {
                            titleParent = "Dixième " + type + " : " + node.Label;
                            break;
                        }
                    default:
                        {
                            titleParent = "Partie " + (node.number) + " : " + node.Label;
                            break;
                        }
                }
            }
            return titleParent;
        }
        public static string getTitlePartie(NodeLawVM node, string type, string lang)
        {
            string titleParent = string.Empty;

            switch (node.number)
            {
                case 1:
                    {
                        titleParent = type + " الأول ";
                        break;
                    }
                case 2:
                    {
                        titleParent = type + "  الثاني ";
                        break;
                    }
            }

            return titleParent;
        }

        public static string getTitleArticle(NodeVmPrint node, string lang)
        {

            string articleNumber = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    articleNumber = "المادة الأولى";
                }
                else
                {

                    articleNumber = "المادة " + (node.number) + " " + getBis(node.bis, lang);
                }
            }
            else
            {
                if (node.number == 1)
                {
                    articleNumber = "Article Premier";
                }
                else
                {
                    articleNumber = "Article " + (node.number) + " " + getBis(node.bis, lang);
                }
            }

            return articleNumber;
        }

        public static string getTitleArticle(E.Loi.DataAccess.Models.Node node, string lang)
        {

            string articleNumber = string.Empty;
            if (lang == "ar")
            {
                if (node.Number == 1)
                {
                    articleNumber = "المادة الأولى";
                }
                else
                {

                    articleNumber = "المادة " + (node.Number) + " " + getBis(node.Bis, lang);
                }
            }
            else
            {
                if (node.Number == 1)
                {
                    articleNumber = "Article Premier";
                }
                else
                {
                    articleNumber = "Article " + (node.Number) + " " + getBis(node.Bis, lang);
                }
            }

            return articleNumber;
        }
        public static string getTitleArticle(NodeLawVM node, string lang)
        {

            string articleNumber = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    articleNumber = "المادة الأولى";
                }
                else
                {

                    articleNumber = "المادة " + (node.number) + " " + getBis(node.bis, lang);
                }
            }
            else
            {
                if (node.number == 1)
                {
                    articleNumber = "Article Premier";
                }
                else
                {
                    articleNumber = "Article " + (node.number) + " " + getBis(node.bis, lang);
                }
            }

            return articleNumber;
        }
        public static string getTitleArticleParent(NodeTitleVM node, string lang)
        {

            string articleNumber = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    articleNumber = "المادة الأولى";
                }
                else
                {

                    articleNumber = "المادة " + (node.number) + " " + getBis(node.bis, lang);
                }
            }
            else
            {
                if (node.number == 1)
                {
                    articleNumber = "Article Premier";
                }
                else
                {
                    articleNumber = "Article " + (node.number) + " " + getBis(node.bis, lang);
                }
            }

            return articleNumber;
        }
        public static string getTitleArticleMetier(NodeVmPrint node, string lang)
        {

            string articleNumber = string.Empty;
            if (lang == "ar")
            {
                articleNumber = "الفصل " + (node.number) + " " + getBis(node.bis, lang);
            }
            else
            {
                articleNumber = "Article " + (node.number) + " " + getBis(node.bis, lang);
            }

            return articleNumber;
        }

        public static string getTitleAlineaImpot(NodeVmPrint node, string lang)
        {
            string title = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    title = $"يتعلق بتغيير وتتميم أحكام بعض مواد {Environment.NewLine} المدونة العامة للضرائب";
                }
                if (node.number == 2)
                {
                    title = $"المتعلق بتتميم المدونة العامة للضرائب {Environment.NewLine} بمادة جديدة";
                }
                if (node.number == 3)
                {
                    title = $"المتعلق بنسخ وتعويض أحكام بعض المواد";
                }
                if (node.number == 4)
                {
                    title = $"المتعلق بالنسخ";
                }
                if (node.number == 5)
                {
                    title = $"المتعلق بدخول حيز التطبيق وأحكام انتقالية";
                }
            }

            return title;
        }
        public static string getTitleAlineaDouane(NodeVmPrint node, string lang)
        {
            string title = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    title = $"يتعلق بتغيير وتتميم أحكام بعض مواد {Environment.NewLine} المدونة العامة للضرائب";
                }
                if (node.number == 2)
                {
                    title = $"المتعلق بتتميم المدونة العامة للضرائب {Environment.NewLine} بمادة جديدة";
                }
                if (node.number == 3)
                {
                    title = $"المتعلق بنسخ وتعويض أحكام بعض المواد";
                }
                if (node.number == 4)
                {
                    title = $"المتعلق بالنسخ";
                }
                if (node.number == 5)
                {
                    title = $"المتعلق بدخول حيز التطبيق وأحكام انتقالية";
                }
            }

            return title;
        }

        public static string getTypeAlinea(NodeVmPrint node, string lang)
        {
            string title = string.Empty;
            if (lang == "ar")
            {
                if (node.number == 1)
                {
                    title = "البند I ";
                }
                if (node.number == 2)
                {
                    title = "البند II";
                }
                if (node.number == 3)
                {
                    title = "البند III";
                }
                if (node.number == 4)
                {
                    title = "البند IV";
                }
                if (node.number == 5)
                {
                    title = "البند V";
                }
            }

            return title;
        }

        public static string getBis(int? i, string lang)
        {
            string bis = String.Empty;
            Dictionary<int?, string> mapArabic = new Dictionary<int?, string> {
                { 0, ""},
                { 1,"مكرّر"},
                { 2,"مكرّر مرّتين"},
                { 3,"مكرّر ثلاث مرات"},
                { 4,"مكرّر أربع مرات"},
                { 5, "مكرّر خمس مرات" }
            };
            Dictionary<int?, string> mapFrench = new Dictionary<int?, string>
            {
                { 0, ""},
                { 1,"bis" },
                { 2,"ter"},
                { 3,"quater"},
                { 4,"quinquies"},
                { 5,"sexies"},
                { 6,"septies"},
                { 7,"octies" },
                { 8,"nonies"},
                { 9,"decies"},
            };


            if (lang == "ar")
            {
                try
                {
                    bis = mapArabic[i];
                }
                catch (KeyNotFoundException)
                {
                    bis = $"مكرّر {i} مرات";
                }
            }
            else if (lang == "fr")
            {
                try
                {
                    bis = mapFrench[i];
                }
                catch (KeyNotFoundException)
                {
                    bis = $"bis {i}eme fois";
                }
            }

            return bis;


        }
        public static string getNumberNode(int i, string lang)
        {
            string number = String.Empty;
            Dictionary<int, string> mapArabic = new Dictionary<int, string> {
                { 0, ""},
                { 1,"الأول"},
                { 2,"الثاني"},
                { 3,"الثالث"},
                { 4,"الرابع"},
                { 5, "الخامس" },
                { 6, "السادس" },
                { 7, "السابع" },
                { 8, "الثامن" },
                { 9, "التاسع" },
                { 10, "العاشر" },
            };
            Dictionary<int, string> mapFrench = new Dictionary<int, string>
            {
                { 0, ""},
                { 1,"première" },
                { 2,"deuxième"},
                { 3,"troisième"},
                { 4,"quatrième"},
                { 5,"cinquième"},
                { 6,"sixième"},
                { 7,"septième" },
                { 8,"huitième"},
                { 9,"neuvième"},
                { 10,"dixième"},

            };


            if (lang == "ar")
            {
                try
                {
                    number = mapArabic[i];
                }
                catch (KeyNotFoundException)
                {
                    number = $" {i} ";
                }
            }
            else if (lang == "fr")
            {
                try
                {
                    number = mapFrench[i];
                }
                catch (KeyNotFoundException)
                {
                    number = $" {i} ";
                }
            }

            return number;


        }


        //TODO retun type en fr
        public static string getNodeType(string nodeType)
        {
            if ((nodeType.Contains("قانون")) || (nodeType.ToUpper().Contains("loi".ToUpper())))
            {
                return "projet";
            }
            else if ((nodeType.Contains("جزء")) || (nodeType.ToUpper().Contains("partie".ToUpper())))
            {
                return "partie";
            }
            else if ((nodeType.Contains("باب")) || (nodeType.ToUpper().Contains("titre".ToUpper())))
            {
                return "chapitre";
            }
            else if ((nodeType.Contains("وظيفية")) || (nodeType.ToUpper().Contains("metier".ToUpper())))
            {
                return "articleMetier";
            }
            else if ((nodeType.Contains("مادة")) || (nodeType.ToUpper().Contains("article".ToUpper())))
            {
                return "article";
            }
            else if ((nodeType.Contains("بند")) || (nodeType.ToUpper().Contains("alinea".ToUpper())))
            {
                return "alinea";
            }
            else if ((nodeType.Contains("فقرة")) || (nodeType.ToUpper().Contains("paragraphe".ToUpper())))
            {
                return "titre";
            }
            else if ((nodeType.Contains("عنوان")) || (nodeType.ToUpper().Contains("groupe".ToUpper())))
            {
                return "groupe";
            }
            else if ((nodeType.Contains("ملحق")) || (nodeType.ToUpper().Contains("annexe".ToUpper())))
            {
                return "annexe";
            }
            else if ((nodeType.Contains("جدول")) || (nodeType.ToUpper().Contains("tableau".ToUpper())))
            {
                return "tableau";
            }
            else
            {
                return "other";
            }
        }


        public static void ConfigBuilder(CustomDocumentBuilder builder, string lang)
        {
            if (lang == "ar")
            {
                builder.MoveToDocumentEnd();
                builder.Font.ClearFormatting();
                builder.Font.NameBi = "Sakkal Majalla";
                builder.Font.SizeBi = 16;
                builder.Font.Bidi = true;
                builder.Font.ItalicBi = false;
                builder.Font.BoldBi = true;
                builder.Font.LocaleIdBi = 1025;
                TextColumnCollection columns = builder.PageSetup.TextColumns;
                columns.LineBetween = true;
                columns.EvenlySpaced = true;
                columns.SetCount(1);
                builder.ParagraphFormat.Bidi = true;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
            //configuration
            builder.ParagraphFormat.Alignment = (lang == "ar" ? ParagraphAlignment.Right
                : ParagraphAlignment.Left);

            PageSetup pagesetup = builder.PageSetup;

            pagesetup.PaperSize = PaperSize.A4;
            pagesetup.LeftMargin = 28;
            pagesetup.RightMargin = 28;
            pagesetup.HeaderDistance = 25;
            pagesetup.PageStartingNumber = 1;
            pagesetup.RestartPageNumbering = false;
        }
        public static void ConfigBuilderLandscape(CustomDocumentBuilder builder, string lang)
        {
            builder.PageSetup.Orientation = Orientation.Landscape;
            if (lang == "ar")
            {
                builder.MoveToDocumentEnd();
                builder.Font.ClearFormatting();
                builder.Font.NameBi = "Sakkal Majalla";
                builder.Font.SizeBi = 16;
                builder.Font.Bidi = true;
                builder.Font.ItalicBi = false;
                builder.Font.BoldBi = true;
                builder.Font.LocaleIdBi = 1025;
                TextColumnCollection columns = builder.PageSetup.TextColumns;
                columns.LineBetween = true;
                columns.EvenlySpaced = true;
                columns.SetCount(1);
                builder.ParagraphFormat.Bidi = true;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            }
            //configuration
            builder.ParagraphFormat.Alignment = (lang == "ar" ? ParagraphAlignment.Justify
                : ParagraphAlignment.Left);

            PageSetup pagesetup = builder.PageSetup;

            pagesetup.PaperSize = PaperSize.A4;
            pagesetup.LeftMargin = 25.00;
            pagesetup.RightMargin = 25.00;
            pagesetup.TopMargin = 15.00;
            pagesetup.BottomMargin = 5.00;
            pagesetup.HeaderDistance = 25;
            pagesetup.PageStartingNumber = 1;
            pagesetup.RestartPageNumbering = false;
        }
        public static void AddGuardPage(CustomDocumentBuilder builder, string title, string loi, string annee, PhaseVM phase, string lang, string version)
        {
            var guardPage = "views/" + lang + "/page_de_garde.docx";
            if (version == "membre")
            {
                guardPage = "views/" + lang + "/page_de_garde_membre.docx";
            }
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkName = docPageDeGarde.Range.Bookmarks["loi"];
            Bookmark bookmarkYear = docPageDeGarde.Range.Bookmarks["annee"];
            Bookmark bookmarkPhase = docPageDeGarde.Range.Bookmarks["phase"];
            Bookmark bookmarkTitle = docPageDeGarde.Range.Bookmarks["title"];

            if (bookmarkName != null)
            {
                bookmarkName.Text = loi;
            }

            if (bookmarkYear != null)
            {
                bookmarkYear.Text = annee;
            }
            if (bookmarkPhase != null)
            {
                bookmarkPhase.Text = title;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }
        public static async void AddTextLawGuarddPage(CustomDocumentBuilder builder, string law)
        {
            var guardPage = "views/TextLaw/TextLaw.docx";
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkLaw = docPageDeGarde.Range.Bookmarks["law"];
            if (bookmarkLaw != null)
            {
                bookmarkLaw.Text = law;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }

        public static async void AddVotingFileGuarddPage(CustomDocumentBuilder builder, string law, string section)
        {
            var guardPage = "views/VotingFileSession/VotingFileSessionGuardPage.docx";
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkLaw = docPageDeGarde.Range.Bookmarks["law"];
            Bookmark bookmarkSection = docPageDeGarde.Range.Bookmarks["section"];
            if (bookmarkLaw != null)
            {
                bookmarkLaw.Text = law;
            }

            if (bookmarkSection != null && section != null)
            {
                bookmarkSection.Text = section;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }

        public static async void AddVoteAmendmentsResultGuarddPage(CustomDocumentBuilder builder, string law)
        {
            var guardPage = "views/Voteresult/PageDeGardVoteAmendmentsResult.docx";
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkLaw = docPageDeGarde.Range.Bookmarks["law"];
            if (bookmarkLaw != null)
            {
                bookmarkLaw.Text = law;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }

        public static async void AddAmendGrpGuardPage(CustomDocumentBuilder builder, string law, string team, bool isVote)
        {
            var guardPage = isVote ? "views/amendments/VoteAmendmentsGuardPage.docx" : string.IsNullOrEmpty(team) ? "views/amendments/AllGuardPage.docx" : "views/amendments/GroupeGuardPage.docx";
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkLaw = docPageDeGarde.Range.Bookmarks["law"];
            Bookmark TeamTeam = docPageDeGarde.Range.Bookmarks["team"];
            if (bookmarkLaw != null)
            {
                bookmarkLaw.Text = law;
            }

            if (TeamTeam != null && team != null)
            {
                TeamTeam.Text = team;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }
        public static void AddAmendmentsGuardPage(CustomDocumentBuilder builder, string law, string language, bool withVote)
        {
            var guardPage = string.Empty;
            if (withVote)
            {
                guardPage = "views/" + language + "/amendments/VoteAmendmentsGuardPage.docx";
            }
            else
            {
                guardPage = "views/" + language + "/amendments/SuiviAmendmentsGuardPage.docx";
            }
            var guardPageDup = Guid.NewGuid().ToString() + ".docx";
            File.Copy(guardPage, guardPageDup);
            var docPageDeGarde = new Aspose.Words.Document(guardPageDup);
            Bookmark bookmarkLaw = docPageDeGarde.Range.Bookmarks["law"];
            if (bookmarkLaw != null)
            {
                bookmarkLaw.Text = law;
            }
            builder.InsertDocument(docPageDeGarde, ImportFormatMode.UseDestinationStyles);
            File.Delete(guardPageDup);
        }
        public static void AddHeaderAndFooterPresentation(CustomDocumentBuilder builder, Document document, LawVM law, PhaseVM phase, string lang, int startingSection)
        {

            builder.MoveToSection(document.Sections.IndexOf(document.Sections[startingSection]));
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            if (lang == "fr")
            {
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                builder.InsertHtml("<span style='font-size:10px;'>" + "Projet de loi de finances " + " " + law.Number + " de l'année " + law.Year + "</span>" + string.Join("", (new string[10]).Select(n => "&nbsp;")));
            }
            else if (phase.order == 3)
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + "تقديم المواد" + string.Join("", (new string[200]).Select(n => "&nbsp;")) + "</span>");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.InsertHtml("<span>" + string.Join("", (new string[65]).Select(n => "&nbsp;")) + "</span>");
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية رقم" + " " + law.Number + " للسنة المالية " + law.Year + " كما وافق عليه مجلس النواب " + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");

            }
            else if (phase.order == 5)
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + "تقديم المواد" + string.Join("", (new string[200]).Select(n => "&nbsp;")) + "</span>");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.InsertHtml("<span>" + string.Join("", (new string[65]).Select(n => "&nbsp;")) + "</span>");
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية رقم" + " " + law.Number + " للسنة المالية " + law.Year + " كما وافق عليه مجلس المستشارين " + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");
            }
            else
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + "تقديم المواد" + string.Join("", (new string[200]).Select(n => "&nbsp;")) + "</span>");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.InsertHtml("<span>" + string.Join("", (new string[85]).Select(n => "&nbsp;")) + "</span>");
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية رقم" + " " + law.Number + " للسنة المالية " + law.Year + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");
            }

            builder.InsertHtml("<div style='width:150%;border-top:1px solid black;height:3px;'>&nbsp;</div>");

            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);

            builder.InsertHtml("<div style='width:150%;border-top:1px solid black;'>");

            if (lang == "fr")
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + "Projet de loi de finances " + " " + law.Number + " de l'année " + law.Year + "</span>" + string.Join("", (new string[60]).Select(n => "&nbsp;")));
                var field = builder.InsertField("PAGE", string.Empty);
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                builder.InsertHtml("<span>" + string.Join("", (new string[100]).Select(n => "&nbsp;")) + "</span>");
                builder.InsertHtml("<span style='font-size:10px;'>Présentation des articles</span>" + string.Join("", (new string[1]).Select(n => "&nbsp;")));
            }
            else
            {
                var field = builder.InsertField("PAGE", string.Empty);
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                builder.InsertHtml("<span>" + string.Join("", (new string[130]).Select(n => "&nbsp;")) + "</span>");
            }

            builder.InsertHtml("</div>");
        }
        public static string RemoveStyle(string html, string style)
        {
            Regex regex = new Regex(style + "\\s*:.*?;?");

            return regex.Replace(html, string.Empty);
        }
        public static string UpdateStyle(string contenu)
        {
            string s = contenu;
            string divContent = string.Empty;
            if (s != "" && s is not null)
            {

                //Regex fontsize = new Regex(@"font-size:(.*?);");
                Regex lineheight = new Regex(@"line-height:(.*?);");
                Regex fontfamily = new Regex(@"font-family:(.*?);");
                Regex textdecoration = new Regex(@"text-decoration-line:(.*?);");
                var tableSize = @"(.*<table.+?(?=width=""))(width="")(.+?(?=""))(""\s*.*>{1}.*)";
                s = s.Replace("…", "...");
                s = s.Replace("…", "...");
                s = s.Replace("…", "...");
                //s = Regex.Replace(s, "font-size:\\s*\".*\";?", string.Empty);

                s = RemoveStyle(s, "font-size");
                //s = RemoveStyle(s, "text-align");


                s = s.Replace("...........", ". . . . . . . . . . . ");
                //s = fontsize.Replace(s, "font-size:20px !important;");
                s = fontfamily.Replace(s, "font-family:sakkal majalla !important;");
                s = textdecoration.Replace(s, "text-decoration-line: line-through !important;");
                s = s.Replace("&nbsp;", " ").Replace("&beta;", "β");
                s = s.Replace("<br>", "");
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                s = regex.Replace(s, " ");



                var splits = s.Split("<table");
                var res = splits[0];
                for (var i = 1; i < splits.Length; i++)
                {

                    var str = "<table" + splits[i];
                    var second_splits = str.Split(">");
                    var balise = second_splits[0] + ">";

                    var tmp = Regex.Replace(balise, tableSize, m =>
                    {
                        return m.Groups[1].Value + m.Groups[2].Value + "100%" + m.Groups[4].Value;
                    }
                    );
                    res += tmp;
                    for (var index = 1; index < second_splits.Length; index++)
                    {
                        res += (index > 1 ? ">" : "") + second_splits[index];
                    }
                }
                s = res;



            }
            return s;
        }
        public static string UpdateStyleOriginal(string contenu, string lang)
        {
            string s = contenu;
            string divContent = string.Empty;
            if (s != "" && s is not null)
            {

                if (lang == "ar")
                {
                    Regex lineheight = new Regex(@"line-height:(.*?);");
                    Regex fontfamily = new Regex(@"font-family:(.*?);");
                    var tableSize = @"(.*<table.+?(?=width=""))(width="")(.+?(?=""))(""\s*.*>{1}.*)";
                    s = s.Replace("…", "...");
                    s = s.Replace("…", "...");
                    s = s.Replace("…", "...");


                    s = s.Replace("...........", ". . . . . . . . . . . ");
                    s = fontfamily.Replace(s, "font-family:sakkal majalla !important;");
                    s = s.Replace("&nbsp;", " ").Replace("&beta;", "β");

                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex("[ ]{2,}", options);
                    s = regex.Replace(s, " ");



                    var splits = s.Split("<table");
                    var res = splits[0];
                    for (var i = 1; i < splits.Length; i++)
                    {
                        var str = "<table" + splits[i];
                        var second_splits = str.Split(">");
                        var balise = second_splits[0] + ">";

                        var j = 0;
                        var tmp = Regex.Replace(balise, tableSize, m =>
                        {
                            return m.Groups[1].Value + m.Groups[2].Value + "100%" + m.Groups[4].Value;
                        }
                        );
                        res += tmp;
                        for (var index = 1; index < second_splits.Length; index++)
                        {
                            res += (index > 1 ? ">" : "") + second_splits[index];
                        }
                    }
                    res = RemoveStyle(res, "font-size");

                    s = res;

                }
                else if (lang == "fr")
                {

                    Regex fontsize = new Regex(@"font-size:(.*?);");
                    Regex lineheight = new Regex(@"line-height:(.*?);");
                    Regex fontfamily = new Regex(@"font-family:(.*?);");
                    var tableSize = @"(.*<table.+?(?=width=""))(width="")(.+?(?=""))(""\s*.*>{1}.*)";
                    s = s.Replace("…", "...");
                    s = s.Replace("…", "...");
                    s = s.Replace("...........", ". . . . . . . . . . . ");

                    s = fontsize.Replace(s, "font-size:16px !important;");
                    s = lineheight.Replace(s, "line-height : 100% !important;");
                    s = fontfamily.Replace(s, "font-family:Times New Roman !important;");
                    var splits = s.Split("<table");
                    var res = splits[0];
                    for (var i = 1; i < splits.Length; i++)
                    {
                        var str = "<table" + splits[i];
                        var second_splits = str.Split(">");
                        var balise = second_splits[0] + ">";

                        Console.WriteLine(i + " / " + (splits.Length - 1));
                        var j = 0;
                        var tmp = Regex.Replace(balise, tableSize, m =>
                        {
                            Console.WriteLine(++j);
                            return m.Groups[1].Value + m.Groups[2].Value + "100%" + m.Groups[4].Value;
                        }
                        );
                        res += tmp;
                        for (var index = 1; index < second_splits.Length; index++)
                        {
                            res += (index > 1 ? ">" : "") + second_splits[index];
                        }
                    }
                    s = res;
                    s = "<div dir=\"LTR\" style=\"font-size:16px;font-family:Times New Roman;line-height:100%;\">" + s + "</div>";
                }
            }
            return s;
        }
        public static string UpdateStyleAnnexe(string contenu, string lang)
        {
            string s = contenu;
            if (s != "")
            {

                if (lang == "ar")
                {

                    Regex fontsize = new Regex(@"font-size:(.*?);");
                    Regex lineheight = new Regex(@"line-height:(.*?);");
                    Regex fontfamily = new Regex(@"font-family:(.*?);");
                    s = fontsize.Replace(s, "font-size:16px !important;");
                    s = lineheight.Replace(s, "line-height: 100% !important;");
                    s = fontfamily.Replace(s, "font-family:sakkal majalla !important;");

                }
                else if (lang == "fr")
                {
                    Regex fontsize = new Regex(@"font-size:(.*?);");
                    Regex lineheight = new Regex(@"line-height:(.*?);");
                    Regex fontfamily = new Regex(@"font-family:(.*?);");

                    s = fontsize.Replace(s, "font-size:12px !important;");
                    s = lineheight.Replace(s, "line-height: 100% !important;");
                    s = fontfamily.Replace(s, "font-family:Times New Roman !important;");

                }
            }
            return s;
        }
        public static void AddHeaderAndFooterAmendment(CustomDocumentBuilder builder, Document document, string lang, int startingSection)
        {
            builder.MoveToSection(document.Sections.IndexOf(document.Sections[startingSection]));
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.InsertHtml("<div style='width:150%;border-top:1px solid black;'>");
            var field = builder.InsertField(FieldType.FieldPage, true);
            if (lang == "fr")
            {
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
            else
            {
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            }
            builder.InsertHtml("</div>");
        }
        public static void AddHeaderAndFooterAmendment(CustomDocumentBuilder builder, Document document, string lang, int startingSection, string teamLabel)
        {
            builder.MoveToSection(document.Sections.IndexOf(document.Sections[startingSection]));
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.InsertHtml("<div style='width:150%;border-top:1px solid black;'>");
            var field = builder.InsertField(FieldType.FieldPage, true);
            if (lang == "fr")
            {
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
            else
            {
                field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                builder.InsertHtml("<span style='font-size:15px;text-align:right'>" + string.Join("", (new string[130]).Select(n => "&nbsp;")) + teamLabel + "</span>");
            }
            builder.InsertHtml("</div>");
        }


    }
}
