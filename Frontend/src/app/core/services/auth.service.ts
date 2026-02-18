import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, of, tap, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = `${environment.apiUrl}/auth`;
    private estaLogueadoSubject = new BehaviorSubject<boolean>(this.tieneToken());
    public estaLogueado$ = this.estaLogueadoSubject.asObservable();
    private csrfToken: string | null = null;

    constructor(private http: HttpClient) {
        // Al iniciar, verificamos si hay sesión válida en backend
        this.verificarSesion().subscribe();
    }

    private tieneToken(): boolean {
        return !!localStorage.getItem('usuario');
    }

    obtenerUsuarioAlmacenado(): string | null {
        return localStorage.getItem('usuario');
    }

    obtenerToken(): string | null {
        return this.csrfToken;
    }

    obtenerTokenCsrf(): Observable<any> {
        return this.http.get<{ token: string }>(`${this.apiUrl}/csrf/token`, { withCredentials: true }).pipe(
            tap(respuesta => {
                this.csrfToken = respuesta.token;
            })
        );
    }

    iniciarSesion(credenciales: { email: string, password: string }): Observable<any> {
        return this.http.post(`${this.apiUrl}/login`, credenciales, { withCredentials: true }).pipe(
            tap((response: any) => {
                if (response.user) {
                    localStorage.setItem('usuario', response.user);
                }
                this.estaLogueadoSubject.next(true);
            })
        );
    }

    registrarUsuario(credenciales: { email: string, password: string }): Observable<any> {
        return this.http.post(`${this.apiUrl}/register`, credenciales);
    }

    cerrarSesion(): Observable<any> {
        return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true }).pipe(
            tap(() => {
                localStorage.removeItem('usuario');
                this.estaLogueadoSubject.next(false);
                this.csrfToken = null;
            })
        );
    }

    verificarSesion(): Observable<boolean> {
        return this.http.get<{ authenticated: boolean, user?: string }>(`${this.apiUrl}/check-session`, { withCredentials: true }).pipe(
            switchMap(respuesta => {
                const isAuthenticated = respuesta.authenticated;
                if (isAuthenticated && respuesta.user) {
                    localStorage.setItem('usuario', respuesta.user);
                    // Si estamos autenticados, obtenemos el token CSRF para futuras peticiones
                    return this.obtenerTokenCsrf().pipe(
                        map(() => {
                            this.estaLogueadoSubject.next(true);
                            return true;
                        }),
                        catchError(() => {
                            // Si falla obtener el token CSRF, igual marcamos como logueado pero podría fallar POST
                            this.estaLogueadoSubject.next(true);
                            return of(true);
                        })
                    );
                } else {
                    localStorage.removeItem('usuario');
                    this.estaLogueadoSubject.next(false);
                    return of(false);
                }
            }),
            catchError(() => {
                localStorage.removeItem('usuario');
                this.estaLogueadoSubject.next(false);
                return of(false);
            })
        );
    }
}
