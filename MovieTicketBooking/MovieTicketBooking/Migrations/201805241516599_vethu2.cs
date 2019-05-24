namespace MovieTicketBooking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vethu2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PaymentDetails", "CardType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PaymentDetails", "CardType", c => c.String(maxLength: 16));
        }
    }
}
