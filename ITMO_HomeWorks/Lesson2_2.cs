using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
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
    internal class Lesson2_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDocument = uiApp.ActiveUIDocument;
            Document document = uiDocument.Document;

            FilteredElementCollector fec01 = new FilteredElementCollector(document, document.ActiveView.Id);

            IList<Pipe> pipeList = fec01.OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<Pipe>()
                .ToList();
            
            double length = 0.0;
            IList<ElementId> eIdList = new List<ElementId>();
            foreach(Pipe pipe in pipeList)
            {
                length += pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                Element element = pipe as Element;
                if(element != null) eIdList.Add(element.Id);
            }

            // Подсветить выбранные элементы
            uiDocument.Selection.SetElementIds(eIdList);

            TaskDialog.Show("info", $"Количество труб: {pipeList.Count} {Environment.NewLine}" +
                $"Общая длина труб: {Math.Round(UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Meters), 2)} м.");

            return Result.Succeeded;
        }
    }
}
