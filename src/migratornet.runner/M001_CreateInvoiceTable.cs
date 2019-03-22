using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace migratornet.runner
{

    [Migration(1)]
    public class M001_CreateInvoiceTable : Migration
    {
        public override void Down()
        {
            Delete.Table("invoices");
        }

        public override void Up()
        {
            Create.Table("invoices")
                .WithColumn("id").AsInt32().PrimaryKey().NotNullable()
                .WithColumn("creation_date").AsDateTime().NotNullable()
                .WithColumn("created_by").AsString().NotNullable()
                .WithColumn("paid").AsBoolean().WithDefaultValue(false)
                .WithColumn("notes").AsString().Nullable();
        }
    }
}
