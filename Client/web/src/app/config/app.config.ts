export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;
    companyUri: string;
    usersUri: string;
    gridDefinitionUri: string;
    supplierUri: string;
    customerUri: string;
    posUri: string;
    ordersUri: string;
    invoiceUri: string;
    shipmentUri: string;
    barcodeUri: string;
    customerInvoiceUri: string;

    initialize() {
        this.apiServerHost = 'http://po.harisons.com/api';

        this.usersUri = '/users';
        this.partsUri = '/parts';
        this.companyUri = '/companies';
        this.gridDefinitionUri = '/users';
        this.supplierUri = '/suppliers'
        this.customerUri = '/customers'
        this.posUri = '/pos';
        this.ordersUri = '/orders';
        this.invoiceUri = '/supplierinvoice';
        this.shipmentUri = '/PackingSlips';
        this.customerInvoiceUri =  '/Invoices';
        this.barcodeUri = 'https://quest.yellow-chips.com/static/barcode.html?';
    }
}

export class GridConstants {
    public readonly Company = 1;
}