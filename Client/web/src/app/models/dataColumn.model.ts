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
    isDateTime: boolean = false;
    isEditableDate: boolean = false;
    isBoolean: boolean = false;
    checkboxText: string = '';
    minWidth: boolean = false;
    customStyling: string = '';
    isDisabled: boolean = false;
    hasAdditionalAction: boolean = false;
    additionalActionName: string = '';
    columnName: string = '';

    constructor(init?: Partial<DataColumn>) {
        Object.assign(this, init);
    }
}

export class DataColumnAction {
    actionText: string = '';
    actionStyle: string = '';
    event: string = '';
    icon: string = '';
    showOnlyIf: string = '';

    private condition(data: any): boolean {
        return this.showOnlyIf ? eval(this.showOnlyIf): true;
    };

    constructor(init?: Partial<DataColumnAction>) {
        Object.assign(this, init);
    }

}