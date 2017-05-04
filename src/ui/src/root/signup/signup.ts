import Vue = require('vue');
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { IVuelidate, ValidationRuleset, Vuelidate, validationMixin } from 'vuelidate';
import { ICommonOptions } from '../../plugins';
import { AuthenticationService } from '../../services';
import { ISignupOptions, IUser } from '../../model';
import { IRouterMixinData } from '../../mixins/mixin-router';
import { StoreTypes } from '../../store';
import { validations, TSignup } from './signup-validate';

@Component({
    mixins: [validationMixin],
    name: 'Signup',
    template: require('./signup.html'),
    validations: validations
})
export class Signup extends Vue {

    private auth: AuthenticationService;

    constructor() {
        super();
    }

    created() {
        this.auth = new AuthenticationService;
    }

    get allowSubmit() {

        let error = this.$v.signup.$error || this.$v.signup.$invalid;
        return !error;
    }

    submit(): void {

        if (this.allowSubmit) {

            let signup = Object.assign({}, this.signup);

            let onSignup = (value: IUser) => {
                this.$store.dispatch(StoreTypes.updateUser, value);
            }

            let onStoreDispatch = (o) => {
                this.$router.push({ name: 'home' });
            };

            this.$common.exec(this.auth.signup(signup))
                .then(onSignup)
                .then(onStoreDispatch);    
        }
    }

    signup: ISignupOptions = {
        confirmPassword: 'P@ssw0rd',
        displayName: null,
        userName: null,
        password: 'P@ssw0rd'
    };

    $common: ICommonOptions;

    $router: IRouterMixinData;

    $store: Store<{}>;

    $v: Vuelidate<TSignup>
}

export default Signup;