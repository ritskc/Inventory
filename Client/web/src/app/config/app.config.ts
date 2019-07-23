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

    initialize() {
        this.apiServerHost = 'https://questapi.yellow-chips.com';

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
    }
}

export class GridConstants {
    public readonly Company = 1;
}