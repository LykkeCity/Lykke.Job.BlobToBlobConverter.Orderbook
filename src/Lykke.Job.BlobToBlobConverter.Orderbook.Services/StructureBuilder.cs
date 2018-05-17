﻿using System.Linq;
using System.Collections.Generic;
using Lykke.Job.BlobToBlobConverter.Common;
using Lykke.Job.BlobToBlobConverter.Common.Abstractions;
using Lykke.Job.BlobToBlobConverter.Orderbook.Core.Domain.OutputModels;

namespace Lykke.Job.BlobToBlobConverter.Orderbook.Services
{
    public class StructureBuilder : IStructureBuilder
    {
        public static string MainContainer => "orderbook";

        public Dictionary<string, string> GetMappingStructure()
        {
            var result = new Dictionary<string, string>
            {
                { MainContainer, OutOrderbook.GetColumnsString() },
            };
            return result;
        }

        public TablesStructure GetTablesStructure()
        {
            return new TablesStructure
            {
                Tables = new List<TableStructure>
                {
                    new TableStructure
                    {
                        TableName = "Orderbook",
                        AzureBlobFolder = MainContainer,
                        Colums = OutOrderbook.GetStructure()
                            .Select(p => new ColumnInfo { ColumnName = p.Item1, ColumnType = p.Item2 })
                            .ToList(),
                    }
                }
            };
        }
    }
}
