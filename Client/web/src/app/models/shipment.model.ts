export class Shipment {

    constructor() {
        this.packingSlipDetails = [];
    }

    CompanyId: number = 0;
    customerId: number = 0;
    customerName: string = '';
    packingSlipNo: string = '';
    shippingDate: string = '';
    shipVia: string = '';
    crates: number = 0;
    boxes: number = 0;
    grossWeight: number = 0;
    shippingCharge: number = 0;
    customCharge: number = 0;
    isPaymentReceived: boolean = false;
    packingSlipDetails: PackingSlipDetail[] = [];
}

export class PackingSlipDetail{
    isBlankOrder: boolean = false;
    orderId: number = 0;
    orderDetailId: number = 0;
    partId: number = 0;
    orderNo: string = '';
    partDescription: string = '';
    qty: number = 0;
    inBasket: boolean = false;
    boxes: number = 0;
}