import { Component } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ConnectionService, ConnectionServiceOptions, ConnectionState } from 'ng-connection-service';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'HealthCheck';

  public isOffline: Observable<boolean>;

  constructor(private connectionService: ConnectionService) {
    const options: ConnectionServiceOptions = {
      enableHeartbeat: true,
      heartbeatUrl: `${environment.baseUrl}api/heartbeat`,
      heartbeatInterval: 10000
    };
    this.isOffline = this.connectionService.monitor(options)
      .pipe(map(state => !state.hasNetworkConnection || !state.hasInternetAccess));
  }
}
