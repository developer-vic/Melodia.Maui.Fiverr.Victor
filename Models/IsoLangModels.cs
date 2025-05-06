using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MelodiaTherapy.Models
{
    public class IsoLangModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Native { get; set; }

        public static IsoLangModel FromJson(IDictionary<string, object> json)
        {
            return new IsoLangModel
            {
                Code = json["code"].ToString(),
                Name = json["name"].ToString(),
                Native = json["native"].ToString()
            };
        }
    }
    public class IsoLangRoot
    {
        public Dictionary<string, string> Items { get; set; }

        public static IsoLangRoot FromJson(IDictionary<string, object> parsedJson)
        {
            var itemsList = parsedJson["items"] as IEnumerable<object>;
            var isoLangsList = itemsList
                .Select(item => IsoLangModel.FromJson(item as IDictionary<string, object>))
                .ToList();

            var itemsDict = isoLangsList.ToDictionary(x => x.Code, x => x.Native);

            return new IsoLangRoot { Items = itemsDict };
        }
    }

}