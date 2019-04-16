using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Farmarcas.Radar.Import.Models.Store;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Farmarcas.Radar.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("File Path:");
            var path = Console.ReadLine();

            var list = ConvertToObject(path)
                .Where(x => !String.IsNullOrEmpty(x.CNPJ)).ToList();
            Console.WriteLine("Sending");
            SendToSave(list);

            var query = GenerateSqlToRun(list);
            Console.Write(query);
            File.WriteAllText("C:/query.sql", query);
        }

        static List<FarmarcasRow> ConvertToObject(string path){
            var list = new List<FarmarcasRow>();
            using(var excel = new ExcelPackage(new FileInfo(path))){
                var sheet = excel.Workbook.Worksheets.FirstOrDefault();
                for (var i = sheet.Dimension.Start.Row + 1; i <= sheet.Dimension.End.Row; i++){
                    var row = new FarmarcasRow();

                    row.RazaoSocial = sheet.Cells[i, 1].GetValue<string>();
                    row.CNPJ = sheet.Cells[i, 2].GetValue<string>();
                    row.PrePedido = sheet.Cells[i, 3].GetValue<string>();

                    list.Add(row);
                }
            }
            return list;
        }

        static StoreModel ConvertToStore(FarmarcasRow row){
            var store = new StoreModel();

            store.CompanyName = row.RazaoSocial;
            store.CNPJ = row.CNPJ;

            store.CommercialStrategy.HasPreOrder = row.PrePedido != null ? row.PrePedido.Equals("SIM") : false;

            return store;
        }

        static void SendToSave(List<FarmarcasRow> list){
            var stores = new List<StoreModel>();
            Console.WriteLine("Converting Obj");
            list.ForEach(x =>
            {
                stores.Add(ConvertToStore(x));
            });
        }
       
        static string GenerateSqlToRun(List<FarmarcasRow> list)
        {
            string query = String.Empty;

            list.ForEach(x =>
            {
                query += $"UPDATE store_commercial_strategy SET Has_Pre_Order = '1' where Id_Store = (SELECT Id FROM store WHERE CNPJ = '{x.CNPJ}');";
                query += "\n";
            });

            return query;
        }
        
        class FarmarcasRow
        {
            public string RazaoSocial { get; set; }
            public string CNPJ { get; set; }
            public string PrePedido { get; set; }
        }
    }
}
