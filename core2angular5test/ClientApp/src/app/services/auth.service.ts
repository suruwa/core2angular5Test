import { EventEmitter, Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import 'rxjs/Rx'
import {Token} from "@angular/compiler";

@Injectable()
export class AuthService {
    authKey: string = "auth";
    clientId: string = "TestMakerFree";

    constructor(private http: HttpClient,
                @Inject(PLATFORM_ID) private platformId: any) {

    }

    // performs the login
    login(username: string, password: string): Observable<boolean> {
        var url = "api/auth/jwt";

        var data = {
            username: username,
            password: password,
            client_id: this.clientId,
            // required when signing up with username/password
            grant_type: "password",
            // space-separated list of scopes for which the token is issued
            scope: "offline_access profile email"
        };

        return this.http.post<TokenResponse>(url, data)
            .map((res) => {
                let token = res && res.token;
                // if the token is there, login has been successful
                if (token) {
                    // store username and jwt token
                    this.setAuth(res);
                    // successful login
                    return true;
                }

                return Observable.throw('Unauthorized');
            }).
            catch(error => {
               return new Observable<any>(error);
            });
    }

    // performs the logout
    logout(): boolean {
        this.setAuth(null);
        return true;
    }

    // Persist auth into localStorage or removes it if a NULL argument is given
    setAuth(auth: TokenResponse | null) : boolean {
        if (isPlatformBrowser(this.platformId)) {
            // isPlatformBrowser - angular function that, if feeded an instance of PLATFORM_ID,
            // obtained through DI, returns true if execution context is a browser
            // this is to check localStorage is accessed only in a browser,
            // server-side rendering doesn't support it
            // this is called isomorphic approach, runs on client-side and server-side
            // applications like these are called Angular Universal apps

            if (auth) {
                localStorage.setItem(this.authKey, JSON.stringify(auth));
            }
            else {
                localStorage.removeItem(this.authKey);
            }
        }

        return true;
    }

    // Retrieves the auth JSON object (or NULL if none)
    getAuth(): TokenResponse | null {
        if (isPlatformBrowser(this.platformId)) {
            var i = localStorage.getItem(this.authKey);

            if (i) {
                return JSON.parse(i);
            }
        }

        return null;
    }

    // Returns TRUE if the user is logged in, FALSE otherwise
    isLoggedIn(): boolean {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(this.authKey) != null;
        }

        return false;
    }
}

