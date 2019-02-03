export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;
    companyUri: string;

    initialize() {
        this.apiServerHost = 'http://po.harisons.com/api';
        this.partsUri = '/parts';
        this.companyUri = '/companies';
    }
}