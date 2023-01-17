using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITMO_HomeWorks
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Lesson2_4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDocument = uiApp.ActiveUIDocument;
            Document document = uiDocument.Document;

            FilteredElementCollector fec_Ducts = new FilteredElementCollector(document);
            
            IList<Duct> ductList = fec_Ducts.OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .Cast<Duct>()
                .OrderBy(it => it.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsValueString()) // Сортировать по имени уровня
                .ToList();

            Dictionary<string, int> dicDuctPerLevels = new Dictionary<string, int>();
            int countPerLevel = 0;  // Количество воздуховодов на этаже
            int totalCount = 0;     // Общее количество воздуховодов, для проверки (см. Lesson2_1)

            foreach(Duct duct in ductList)
            {
                ElementId  eiD = duct.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId();
                string nameLevel = document.GetElement(eiD).Name;   //Получить имя уровня для воздуховода duct

                if (!dicDuctPerLevels.ContainsKey(nameLevel))
                {
                    // Сбросить значения для нового подсчета
                    countPerLevel = 0;
                    // Занести новый уровень в список
                    dicDuctPerLevels.Add(nameLevel, ++countPerLevel);
                    totalCount++;
                }
                else
                {
                    dicDuctPerLevels[nameLevel] = ++countPerLevel;
                    totalCount++;
                }
            }

            string info = string.Empty;
            List<string> levels = new List<string>();

            foreach(KeyValuePair<string, int> i in dicDuctPerLevels)
            {
                info += $"Уровень: {i.Key}, воздуховодов: {i.Value} {Environment.NewLine}";

                //Добавить уровень для окна
                levels.Add(i.Key);
            }

            info += $"Общее количество воздуховодов: {totalCount} {Environment.NewLine}";
            TaskDialog.Show("Info", info);

            return Result.Succeeded;
        }
    }
}
