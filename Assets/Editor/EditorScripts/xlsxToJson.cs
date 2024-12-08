using UnityEngine;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;




public class xlsxToJson : MonoBehaviour
{
    private void Start()
    {

    }




    private static void StatJson() // 将方法改为静态
    {
        string excelPath1 = Path.Combine(Application.dataPath, "Editor", "Words.xlsx");
        string jsonPath1 = Path.Combine(Application.streamingAssetsPath, "Words.json");

        if (File.Exists(excelPath1))
        {
            List<ListData> tableData = ReadExcelFile(excelPath1);
            SaveToJson(tableData, jsonPath1);
        }
    }

    private static List<ListData> ReadExcelFile(string filePath)
    {
        List<ListData> DataList = new List<ListData>();

        FileInfo file = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(file))
        {
            foreach (var worksheet in package.Workbook.Worksheets)
            {

                int colCoint = worksheet.Dimension.End.Column;

                for (int i = 1; i <= colCoint; i++)
                {
                    int rowCount = worksheet.Dimension.End.Row;

                    ListData listData = new ListData();
                    
                    List<string> hh = new List<string>();

                    for (int j = 1; j <= rowCount; j++)
                    {
                      

                        if (worksheet.Cells[j, i].Value != null) {

                            if ( j == 1) {
                                listData.name = worksheet.Cells[j, i].Value.ToString();
                            }
                            else {
                                hh.Add(worksheet.Cells[j, i].Value.ToString());
                            }
                            
                        }

                       
                    }

                    listData .SetData(hh);

                    DataList.Add(listData);

                }

              

            }
;
        }

        return DataList;
    }

    private static void SaveToJson(List<ListData> tableData, string filePath) // 将方法改为静态
    {
        StringList Data = new StringList();

        Data.items = tableData;

        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(filePath, json);
        Debug.Log("JSON file saved to: " + filePath);
    }

    public class CreatSampleExcelAsset : Editor
    {
        // 在Editor界面添加一个MenuItem来执行上面的代码
        [MenuItem("BuildJson/Build ExcelJson")]
        public static void BuildExcelSampleAsset()
        {
            StatJson(); // 调用静态方法
            // 刷新Assets
            AssetDatabase.Refresh();
        }
    }
}
