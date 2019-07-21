export class Shipment {

    constructor() {
        this.PackingSlipDetails = [];
    }

    CompanyId: number = 0;
    CustomerId: number = 0;
    PackingSlipNo: string = '';
    ShippingDate: string = '';
    ShipVia: string = '';
    Crates: number = 0;
    Boxes: number = 0;
    GrossWeight: number = 0;
    ShippingCharge: number = 0;
    CustomCharge: number = 0;
    IsPaymentReceived: boolean = false;
    PackingSlipDetails: PackingSlipDetail[] = [];
}

export class PackingSlipDetail{
    IsBlankOrder: boolean = false;
    OrderId: number = 0;
    OrderDetailId: number = 0;
    PartId: number = 0;
    OrderNumber: string = '';
    PartDescription: string = '';
    Qty: number = 0;
    InBasket: boolean = false;
}