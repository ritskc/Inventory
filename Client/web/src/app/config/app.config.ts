export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;
    companyUri: string;
    usersUri: string;

    initialize() {
        this.apiServerHost = 'http://po.harisons.com/api';

        this.usersUri = '/users';
        this.partsUri = '/parts';
        this.companyUri = '/companies';
    }
}