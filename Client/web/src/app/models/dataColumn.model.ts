export class DataColumn {
    headerText: string = '';
    value: string = '';
    isLink: boolean = false;
    sortable: boolean = false;
    rightAlign: boolean = false;

    constructor(init?: Partial<DataColumn>) {
        Object.assign(this, init);
    }
}