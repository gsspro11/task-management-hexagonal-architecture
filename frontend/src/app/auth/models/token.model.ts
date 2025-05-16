import { JwtHelperService } from '@auth0/angular-jwt';

export class TokenModel {
    id: string;
    email: string;

    constructor(encoded: string) {
        const helper = new JwtHelperService();
        const decodedToken = helper.decodeToken(encoded);
        this.id = decodedToken.sub;
        this.email = decodedToken.email;
    }
}