import Vue = require('vue');
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { Formatter } from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { IUser, UserRoles } from '../../model';
import { Debounce, GlobalConfig } from '../../common';
import { AuthenticationService } from '../../services';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { RouteNames } from '../routes';
import { IRootStoreState, RootStoreTypes } from '../store';

@Component({
  template: require('./navigation.html')
})
export class AreaNavigation extends Vue {

  private auth: AuthenticationService;

  @State((state: IRootStoreState) => state.common.user) user: IUser;

  changeLocale(lang: string, e: Event) {

    if (lang !== this.$lang) {

      this.$store.dispatch(RootStoreTypes.common.updateLocale, lang)
        .then(() => this.$router.replace(this.$route.path))
        .catch(e => this.$store.dispatch(RootStoreTypes.common.updateStatusBar, e));
    }
  }

  created() {
    this.auth = new AuthenticationService();
  }

  get isInAdminRole() {

    return this.user.authenticated && this.user.roles && this.auth.isInRole(this.user, UserRoles.Admin);
  }

  get locales() {

    return SupportedLocales;
  }

  logout(e: Event) {

    this.auth.logout()
      .then(user => {
        this.$store.dispatch(RootStoreTypes.common.updateUser, user);
      })
      .then(() => {
        let back = window.history.length;
        window.history.go(back);
        window.location.replace(GlobalConfig.uri.site);
      });
  }

  showSearchInput() {

    this.searchOptions.showInput = !this.searchOptions.showInput;
  }

  get searchPageIsActive(): boolean {

    return (this.$route.name === RouteNames.search);
  }

  submitSearch(e: Event) {

    let onSubmitSearch = () => {

      this.searchOptions.showInput = false;

      this.$router.push({
        name: RouteNames.search,
        params: {
          searchText: this.searchOptions.searchText
        }
      });
    }

    Debounce(onSubmitSearch, 500)();
  }

  $lang: string

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $store: Store<IRootStoreState>;

  $t: Formatter

  searchOptions = {
    searchText: '',
    showInput: false
  }

}

export default AreaNavigation;