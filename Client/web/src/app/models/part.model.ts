
export class Part {

    constructor() {
        this.partSupplierAssignments = [];
        this.partCustomerAssignments = [];
    }

    id: any = 0;
    code: string;
    description: string;
    companyId: number = 0;
    weightInKg: number = 0;
    weightInLb: number = 0;
    openingQty: number = 0;
    minQty: number = 0;
    maxQty: number = 0;
    drawingNo: number = 0;
    drawingUploaded: boolean;
    drawingFileName: string;
    location: string;
    isActive: boolean = true;
    isSample: boolean;
    safeQty: number = 0;
    qtyInHand: number = 0;
    intransitQty: number = 0;
    openOrderQty: number = 0;
    supplierOpenPoQty: number = 0;
    supplierCode: string = '';
    isRepackage: boolean;
    
    partSupplierAssignments: PartSupplierAssignment[];
    partCustomerAssignments: PartCustomerAssignment[];
}

export class PartSupplierAssignment
{
    id: number = 0;
    supplierID: number = 0;
    mapCode: string;
    description: string;
    qtyInHand: number = 0;
    qtyInTransit: number = 0;
    totalQty: number = 0;
    unitPrice: number = 0;
}

export class PartCustomerAssignment
{
    id: number = 0;
    customerId: number = 0;
    mapCode: string;
    description: string;
    weight: number = 0;
    rate: number = 0;
    surchargePerPound: number = 0;
    openingQty: number = 0;
    surchargeExist: boolean;
}

export class PartsViewModel {

    constructor(private part: Part) {

    }

    get Code(): string {
        return this.part.code;
    }

    get Description(): string {
        return this.part.description;
    }

    get MinQty(): number {
        return this.part.minQty;
    }

    get MaxQty(): number {
        return this.part.maxQty;
    }

    get SafeQty(): number {
        return this.part.safeQty;
    }

    get IntransitQty(): number {
        return this.part.intransitQty;
    }

    get QuantityInHand(): number {
        return this.part.qtyInHand + this.part.openingQty;
    }

    get OpeningQty(): number {
        return this.part.openingQty;
    }

    set OpeningQty(value: number) {
        this.part.openingQty = value;
    }

    get Total(): number {
        return this.part.openingQty + this.part.qtyInHand + this.part.intransitQty;
    }

    get OpenOrderQty(): number {
        return this.part.openOrderQty;
    }

    get SupplierOpenPoQty(): number {
        return this.part.supplierOpenPoQty;
    }
}