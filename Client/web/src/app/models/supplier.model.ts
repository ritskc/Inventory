import { Term } from './terms.model'

export class Supplier {

    constructor() {
        this.terms = [];
    }

    id: number;
    companyId: number;
    name: string;
    contactPersonName: string;
    phoneNo: string;
    emailID: string;
    address: string;
    city: string;
    state: string;
    country: string;
    zipCode: string;
    faxNo: string;
    dateFormat: Date;
    noofstages: number;
    companyProfileID: number;
    terms: Term[];
}