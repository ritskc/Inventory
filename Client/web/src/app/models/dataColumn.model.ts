export class DataColumn {
    headerText: string = '';
    value: string = '';
    isLink: boolean = false;
    sortable: boolean = false;
    rightAlign: boolean = false;
    isActionColumn: boolean = false;
    isEditable: boolean = false;
    actions: DataColumnAction[] = [];
    nested:string = '';
    constantText: string = '';
    isDate: boolean = false;
    isBoolean: boolean = false;
    minWidth: boolean = false;
    customStyling: string = '';

    constructor(init?: Partial<DataColumn>) {
        Object.assign(this, init);
    }
}

export class DataColumnAction {
    actionText: string = '';
    actionStyle: string = '';
    event: string = '';
    icon: string = '';

    constructor(init?: Partial<DataColumnAction>) {
        Object.assign(this, init);
    }
}