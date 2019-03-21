export class Customer {

    constructor() {
        this.shjippingInfos = [];
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
    shipVia: string;
    invoicingtypeid: number;
    endCustomerName: string;
    displayLineNo: boolean;
    shjippingInfos: ShippingInfo[];
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