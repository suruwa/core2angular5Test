import { Injectable, Injector } from "@angular/core";
import { Router} from "@angular/router";
import {
    HttpClient,
    HttpHandler, HttpEvent, HttpInterceptor,
    HttpRequest, HttpResponse, HttpErrorResponse
} from "@angular/common/http";
import { AuthService } from "./auth.service";
import { Observable } from "rxjs";

@Injectable()
export class AuthResponseInterceptor implements HttpInterceptor {
    currentRequest: HttpRequest<any>;
    auth: AuthService;

    constructor(
        private injector: Injector,
        private router: Router
    ) {

    }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ) : Observable<HttpEvent<any>> {

        this.auth = this.injector.get(AuthService);
        var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;

        //if (token && !request.url.includes("api/auth/jwt")) {// ignore refresh token requests
        if (token) { // used fix from https://github.com/PacktPublishing/ASP.NET-Core-2-and-Angular-5/issues/8
            // save current request
            this.currentRequest = request;

            return next.handle(request)
                .do((event: HttpEvent<any>) => {
                    if (event instanceof HttpResponse) {
                        //do nothing
                    }
                })
                .catch(err => {
                    return this.handleError(err, next);
                });
        } else {
            return next.handle(request);
        }
    }

    handleError(err: any, next: HttpHandler) {
        if (err instanceof HttpErrorResponse) {
            if (err.status == 401) {
                // JWT token may be expired
                // try to get a new one using refresh token
                console.log("Token expired. Attempting refresh...");
                var previousRequest = this.currentRequest;

                return this.auth.refreshToken()
                    .flatMap((refreshed) => {
                        //if (res) {
                            // refresh token successful
                            console.log("refresh token successful");

                            var token = (this.auth.isLoggedIn()) ? this.auth.getAuth()!.token : null;
                            if (token) {
                                previousRequest = previousRequest.clone({
                                    setHeaders: { Authorization: `Bearer ${token}` }
                                });
                                console.log("header token reset");
                            }
                            return next.handle(previousRequest);

                            // // re-submit the failed request
                            // var http = this.injector.get(HttpClient);
                            // http.request(previousRequest)
                            //     .subscribe(res => {
                            //         // do something ???
                            //     }, error1 => console.error(error1));
                        // } else {
                        //     // refresh token failed
                        //     console.log("refresh token failed");
                        //
                        //     // erase current token
                        //     this.auth.logout();
                        //
                        //     // redirect to login page
                        //     this.router.navigate(["login"]);
                        // }
                    });//, error1 => console.log(error1));
            }
        }

        return Observable.throw(err);
    }
}
