export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;

    initialize() {
        this.apiServerHost = 'http://po.harisons.com/api';
        this.partsUri = '/parts';
    }
}