export class Company {
    id: number = 0;
    name: string = '';
    address: string = '';
    phoneNo: string = '';
    faxNo: string = '';
    eMail: string = '';
    contactPersonName: string = '';
    whName: string = '';
    whAddress: string = '';
    whPhoneNo: string = '';
    whEmail: string = '';
    warehouses: Warehouse[] = [];

    constructor(init?: Partial<Company>) {
        Object.assign(this, init);
    }
}

export class Warehouse {
    id: number = 0;
    name: string = '';
}