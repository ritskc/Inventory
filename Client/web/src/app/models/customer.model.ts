export class Customer {

    constructor() {
        this.shippingInfos = [];
        this.shippingInfos.push(new ShippingInfo());
    }

    id: number = 0;
    companyId: number;
    name: string;
    addressLine1: string;
    city: string;
    state: string;
    zipCode: string;
    contactPersonName: string;
    telephoneNumber: string;
    faxNumber: string;
    emailAddress: string;
    truckType: string;
    collectFreight: string;
    comments: string;
    surcharge: number;
    fob: string;
    terms: string;
    rePackingCharge: number;
    rePackingPoNo: string = ''
    shipVia: string;
    invoicingtypeid: number;
    endCustomerName: string;
    displayLineNo: boolean;
    billing: string;
    shippingInfos: ShippingInfo[];
}

export class ShippingInfo {
    id: number = 0;
    customerID: number;
    name: string;
    contactPersonName: string;
    addressLine1: string;
    city: string;
    state: string;
    zipCode: string;
    isDefault: boolean;
}