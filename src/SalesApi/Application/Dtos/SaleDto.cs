using System;
using System.Collections.Generic;

namespace SalesApi.Application.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Cancelled { get; set; }
        public List<SaleItemDto> Items { get; set; }
    }
}
