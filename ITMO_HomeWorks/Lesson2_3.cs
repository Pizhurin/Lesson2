using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITMO_HomeWorks
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Lesson2_3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDocument = uiApp.ActiveUIDocument;
            Document document = uiDocument.Document;


            FilteredElementCollector fec_01 = new FilteredElementCollector(document);

            IList<ElementId> fiListId = fec_01.OfCategory(BuiltInCategory.OST_Columns)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Select(it => it.Id as ElementId)
                .ToList();

            //Подсветить колонны, которые попали в подсчет
            uiDocument.Selection.SetElementIds(fiListId);

            TaskDialog.Show("Info", $"Общее число колонн в модели: {fiListId.Count} шт.");

            return Result.Succeeded;
        }
    }
}
