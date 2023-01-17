using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;

namespace ITMO_HomeWorks
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Lesson2_1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document document = uiDoc.Document;


            //Поиск экземпляров воздуховодов в проекте
            FilteredElementCollector fec01 = new FilteredElementCollector(document);
            double length = 0.0;

            IList<Duct> ductsList = fec01.OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .Cast<Duct>()
                .ToList();
            
            IList<ElementId> eIdList = new List<ElementId>();
            foreach(Duct duct in ductsList )
            {
                length += duct.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                Element e = duct as Element;
                if(e != null) eIdList.Add(e.Id);
            }

            // Подсветить выбранные элементы
            uiDoc.Selection.SetElementIds(eIdList);

            TaskDialog.Show("Info", $"Количество воздуховодов: {ductsList.Count.ToString()}, " +
                $"{Environment.NewLine}Общая длина воздуховодов: {Math.Round(UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Meters), 2)} м.");

            //// Выбор всех воздуховодов через выделение в проекте
            //try
            //{
            //    IList<Reference> refList = uiDoc.Selection.PickObjects(ObjectType.Element, new FilterDuct());
            //    TaskDialog.Show("Info", refList.Count.ToString());
            //}
            //catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            //{
            //}

            // Поиск типов воздуховодов в проекте
            FilteredElementCollector fec02= new FilteredElementCollector(document);
            IList<DuctType> ductTypeList = fec02.OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsElementType()
                .Cast<DuctType>()
                .ToList();
            string typesDuct = string.Empty;
            int count = 0;
            foreach(DuctType dt in ductTypeList )
            {
                typesDuct += $"{++count}. Name: {dt.FamilyName} - {dt.Name} {Environment.NewLine}";
            }
            typesDuct += $"Общее количество типов: {ductTypeList.Count}";
            TaskDialog.Show("Info", typesDuct);

            return Result.Succeeded;
        }
    }
}
