using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace migratornet.runner
{

    [Migration(2)]
    public class M002_CreateInvoiceItemsTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey("fk_items_to_invoice").OnTable("invoice_items");
            Delete.Table("invoice_items");
        }

        public override void Up()
        {
            Create.Table("invoice_items")
                .WithColumn("id").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("part_code").AsString().NotNullable()
                .WithColumn("part_description").AsString().NotNullable()
                .WithColumn("unitary_price").AsDouble()
                .WithColumn("quantity").AsDouble()
                .WithColumn("total_price").AsDouble()
                .WithColumn("invoice_id").AsInt32().ForeignKey("fk_items_to_invoice", "invoices", "id");
        }
    }
}
