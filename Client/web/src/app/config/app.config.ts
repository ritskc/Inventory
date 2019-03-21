export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;
    companyUri: string;
    usersUri: string;
    gridDefinitionUri: string;
    supplierUri: string;

    initialize() {
        this.apiServerHost = 'https://questapi.yellow-chips.com';

        this.usersUri = '/users';
        this.partsUri = '/parts';
        this.companyUri = '/companies';
        this.gridDefinitionUri = '/users';
        this.supplierUri = '/suppliers'
    }
}

export class GridConstants {
    public readonly Company = 1;
}