class DataBindPage {
    static baseUrl = AppHost + '/api/binding/';
    static cls_catch = 'catch';
    static model_id = "modal_tip";
    static urlDel = DataBindPage.baseUrl + 'del';
    static urlsv = DataBindPage.baseUrl + 'save';
    static urlLoadBinding = DataBindPage.baseUrl + 'loadbinding'

    constructor(menu) {
        this.BdModel = new BindingModel(menu);
        this.HttpClient = new HttpAction('databinding');
        this.arrUpperNos = [];
    }

    static showResultModal = function (success, msg, cls) {
        let Dialog = new OperResultDialog(success, msg, cls);
        Dialog.popUp()
    }

    static ckUpper = function (dx) {
        if (dx === 0) {
            return true;
        } else {
            let txt = '';
            for (let i = 0; i < dx; i++) {
                let act = $('section').eq(i).find('a.active').html();
                if (!act) {
                    if (i !== dx - 1)
                        txt += $('section').eq(i).find('div').eq(0).text() + '=>';
                    else {
                        txt += $('section').eq(i).find('div').eq(0).text();
                    }
                } else {
                    continue;
                }
            }

            if (txt !== '') {
                let op = new OperResultDialog(false, '请选择:' + txt, OperResultDialog.ENV_Class.danger)
                op.popUp()
                return false;
            }
            return true;
        }
    }

    static getSecIndex = function (x) { //get  index till the clicked row.

        for (let i = 0; i < $('div.bd').length; i++) {
            if ($('div.bd').eq(i).attr('name') === x) {
                return i;
            }
        }
    }

    init = function () {
        let _slf = this,
            sct = $('section');

        if (sct.length == 0) {
            console.log('[**error**] no section found');
            return;
        }

        $('section a').on('click', function () {
            let f = this;
            _slf.dataClk(f);
        });
        $('.delBind')
            .on('click', function () {
                let e = this;
                _slf.delBinding(e)
            })
            .on('mouseout',
                function (v) {
                    $('.ms').slideUp();
                });
        $('.clrScreen').on('click', _slf.clrScreen);
        $('.sv').on('click', function () {
            let e = this;
            _slf.saveBind(e)
        })
            .on('mouseout',
                function (v) {
                    $('.ms').slideUp();
                });
    }

    clrScreen = function () {
        $('section a').removeClass('active').removeClass(DataBindPage.cls_catch);
    }

    dataClk = function (e) {

        if (this.BdModel.menu != '数据关联') {
            return;
        }

        let
            last_a = '',
            cls = $(e).parents('div').attr('name'),
            indx = DataBindPage.getSecIndex(cls),
            uperOk = DataBindPage.ckUpper(indx);

        this.BdModel.clk = $(e).attr("rel");

        $('section:gt(' + indx + ')').find('a')
            .removeClass('active')
            .removeClass(DataBindPage.cls_catch);

        if (uperOk || indx == 0) {
            if (indx !== 3) {
                $(e).parents('li')
                    .siblings().find('a')
                    .removeClass('active');
            }
            $(e).toggleClass('active');
            this.arrUpperNos = this.BdModel.getActiveRel(indx);

            last_a = $('section')
                .eq(indx)
                .find('a.active')
                .attr('rel');
        }

        if (uperOk && indx < 3) {  //load next grade.             
            this._loadNext(this.arrUpperNos, last_a, 1);
        }
    }

    _loadNext = function (arr, last_a, type) {
        let bindingDto = {
            'Type': type,
            'Selected_Id': last_a,
            'Upper_Ids': arr
        };

        this.HttpClient.requests(DataBindPage.urlLoadBinding, bindingDto,
            r => this.setCatched(r),
            'post');
    }

    /**
     * delete binding data
     * @param {ele clicked} e 
     */
    delBinding = function (e) {
        let last_act = $('section:last').find('a.active');
        if (last_act.length > 0) {
            this.BdModel.getModel();

            this._getLastAct();

            if (this.BdModel.isValid()) {
                this.HttpClient.requests(DataBindPage.urlDel, this.BdModel, x => {

                    let msg = '删除了' + x + '条记录';
                    if (parseInt(x) > 0) {
                        $('section a').removeClass('active')
                            .removeClass(DataBindPage.cls_catch);
                    } else {
                        msg = "有关联课程,没有删除任何关联"
                    }

                    DataBindPage.showResultModal(true, msg);

                }, 'post')
            } else {
                console.log('not valid model');

            }
        } else {
            DataBindPage.ckUpper(4);
        }

    }

    _getLastAct = function () {
        let last_act = $('section:last').find('a.active');
        this.BdModel.last_ids = [];
        for (let index = 0; index < last_act.length; index++) {
            const element = last_act[index];
            this.BdModel.last_ids.push($(element).attr('rel'))
        }
    }

    saveBind = function () {

        let _slf = this,
            isValid = DataBindPage.ckUpper(4);
        if (isValid) {
            _slf.BdModel = _slf.BdModel.getModel();
            _slf._getLastAct();

            if (_slf.BdModel.isValid()) {
                _slf.HttpClient.requests(DataBindPage.urlsv, _slf.BdModel, x => {
                    if (parseInt(x)) {
                        let msg = '保存了' + x + "条记录";
                        DataBindPage.showResultModal(true, msg);
                    }
                }, 'post')
            } else {
                console.log('[binding error]:not valide');
            }
        }
    }


    setCatched = function (rsl) {  //set new clss to ele
        if (rsl) {
            let key = rsl.key, value = rsl.value;
            if (value && value.length > 0) {
                for (let i = 0; i < value.length; i++) {
                    $('div.bd').find('a[rel=' + key + '_' + value[i] + ']')
                        .addClass(DataBindPage.cls_catch);
                }

            }
        }

    }
}



