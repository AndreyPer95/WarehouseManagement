using WarehouseManagement.Models.Resources;
using WarehouseManagement.Models.Units;
using WarehouseManagement.Models.Warehouse;
using WarehouseManagement.Models.Receipts;

namespace WarehouseManagement.Data
{
    public static class DbInitializer
    {
        public static void Initialize(WarehouseContext context)
        {
            if (context.Resources.Any())
            {
                return;
            }
            
            var units = new Unit[]
            {
                new Unit { Name = "шт", Status = UnitStatus.Active },
                new Unit { Name = "кг", Status = UnitStatus.Active },
                new Unit { Name = "л", Status = UnitStatus.Active },
                new Unit { Name = "м", Status = UnitStatus.Active },
                new Unit { Name = "упак", Status = UnitStatus.Active },
                new Unit { Name = "т", Status = UnitStatus.Active },
                new Unit { Name = "м²", Status = UnitStatus.Active },
                new Unit { Name = "м³", Status = UnitStatus.Active }
            };
            context.Units.AddRange(units);
            context.SaveChanges();
            
            var resources = new Resource[]
            {
                new Resource { Name = "Гвозди 100мм", Status = ResourceStatus.Active },
                new Resource { Name = "Доска обрезная 50x200", Status = ResourceStatus.Active },
                new Resource { Name = "Цемент М500", Status = ResourceStatus.Active },
                new Resource { Name = "Песок строительный", Status = ResourceStatus.Active },
                new Resource { Name = "Кирпич красный", Status = ResourceStatus.Active },
                new Resource { Name = "Арматура 12мм", Status = ResourceStatus.Active },
                new Resource { Name = "Щебень гранитный", Status = ResourceStatus.Active },
                new Resource { Name = "Краска водоэмульсионная", Status = ResourceStatus.Active },
                new Resource { Name = "Плитка керамическая", Status = ResourceStatus.Active },
                new Resource { Name = "Провод ВВГ 3x2.5", Status = ResourceStatus.Active }
            };
            context.Resources.AddRange(resources);
            context.SaveChanges();
            
            var receipts = new Receipt[]
            {
                new Receipt 
                { 
                    Number = "ПСТ-000001",
                    Date = DateTime.Now.AddDays(-30)
                },
                new Receipt 
                { 
                    Number = "ПСТ-000002",
                    Date = DateTime.Now.AddDays(-20)
                },
                new Receipt 
                { 
                    Number = "ПСТ-000003",
                    Date = DateTime.Now.AddDays(-10)
                }
            };
            context.Receipts.AddRange(receipts);
            context.SaveChanges();
            
            var receiptResources = new ReceiptResource[]
            {
                new ReceiptResource 
                { 
                    ReceiptId = receipts[0].Id,
                    ResourceId = resources[0].Id,
                    UnitId = units[1].Id,
                    Quantity = 50
                },
                new ReceiptResource 
                { 
                    ReceiptId = receipts[0].Id,
                    ResourceId = resources[1].Id,
                    UnitId = units[7].Id,
                    Quantity = 10
                },
                
                new ReceiptResource 
                { 
                    ReceiptId = receipts[1].Id,
                    ResourceId = resources[2].Id,
                    UnitId = units[1].Id,
                    Quantity = 1000
                },
                new ReceiptResource 
                { 
                    ReceiptId = receipts[1].Id,
                    ResourceId = resources[3].Id,
                    UnitId = units[5].Id,
                    Quantity = 5
                },

                new ReceiptResource 
                { 
                    ReceiptId = receipts[2].Id,
                    ResourceId = resources[4].Id,
                    UnitId = units[0].Id,
                    Quantity = 5000
                },
                new ReceiptResource 
                { 
                    ReceiptId = receipts[2].Id,
                    ResourceId = resources[5].Id,
                    UnitId = units[3].Id,
                    Quantity = 200
                },
                new ReceiptResource 
                { 
                    ReceiptId = receipts[2].Id,
                    ResourceId = resources[8].Id,
                    UnitId = units[6].Id,
                    Quantity = 100
                }
            };
            context.ReceiptResources.AddRange(receiptResources);
            context.SaveChanges();
            
            UpdateWarehouseBalance(context);
        }

        private static void UpdateWarehouseBalance(WarehouseContext context)
        {
            var receiptGroups = context.ReceiptResources
                .GroupBy(rr => new { rr.ResourceId, rr.UnitId })
                .Select(g => new 
                { 
                    g.Key.ResourceId, 
                    g.Key.UnitId, 
                    TotalQuantity = g.Sum(rr => rr.Quantity) 
                })
                .ToList();

            foreach (var group in receiptGroups)
            {
                var balance = context.WarehouseBalances
                    .FirstOrDefault(wb => wb.ResourceId == group.ResourceId && wb.UnitId == group.UnitId);

                if (balance == null)
                {
                    balance = new WarehouseBalance
                    {
                        ResourceId = group.ResourceId,
                        UnitId = group.UnitId,
                        Quantity = group.TotalQuantity
                    };
                    context.WarehouseBalances.Add(balance);
                }
                else
                {
                    balance.Quantity = group.TotalQuantity;
                }
            }

            context.SaveChanges();
        }
    }
}