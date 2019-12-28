export class Shipment {

    constructor() {
        this.packingSlipDetails = [];
    }

    id: number = 0;
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
    fob: string = '';
    totalSurcharge: number = 0;
    terms: string = '';
    shipmentInfoId: number = 0;
    isInvoiceCreated: boolean;
}

export class PackingSlipDetail {
    isBlankOrder: boolean = false;
    orderId: number = 0;
    orderDetailId: number = 0;
    partId: number = 0;
    partCode: string = '';
    orderNo: string = '';
    partDescription: string = '';
    qty: number = 0;
    inBasket: boolean = false;
    boxes: number = 0;
    unitPrice: number = 0;
    total: number = 0;
    surcharge: number = 0;
    surchargePerPound: number = 0;
    surchargePerUnit: number = 0;
    totalSurcharge: number = 0;
    excessQty: number = 0;
    srNo: number = 0;
}