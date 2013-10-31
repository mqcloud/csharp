using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using ProtoBuf.Meta;

namespace MQCloud.Transport.Protocol.Utilities {
    public class SchemaPrinter {
        private static readonly RuntimeTypeModel Model=RuntimeTypeModel.Default;

        private static List<Type> GetTypes() {
            var result=new List<Type>();
            var assembly=Assembly.GetAssembly(typeof(Message));
            var types=assembly.GetTypes();
            foreach (var t in types) {
                var attrs=System.Attribute.GetCustomAttributes(t);
                if (attrs.Any(a => a is ProtoBuf.ProtoContractAttribute)) {
                    result.Add(t);
                }
            }
            return result;
        }

        private static string PrepareSchema(string schema) {
            var result=schema;
            var matches=Regex.Matches(result, @"message\s+\w+_\w+\s+\{[^}]*\}", RegexOptions.Singleline);
            return matches
                .Cast<Match>()
                .Aggregate(result, (current, match)
                    => current.Replace(match.Value, ""));
        }

        public static string Print() {
            var types=GetTypes();
            types.ForEach(t => Model.Add(t, true));

            var schema=Model.GetSchema(null);
            return PrepareSchema(schema);
        }
    }
}