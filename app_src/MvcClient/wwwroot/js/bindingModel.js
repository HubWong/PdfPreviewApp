class BasicBind {
    constructor(lx = -1, mk = -1, xk = -1, bb = []) {
        this.leixing_id = lx;
        this.mokuai_id = mk;
        this.xueke_id = xk;
        this.last_ids = bb;
    }

    createModel(attr_rel) {
        if (attr_rel.indexOf('_') === -1) {
            return;
        }

        let s = attr_rel.split('_');
        switch (s[0]) {
            case 'lx':
                this.leixing_id = s[1]
                break;
            case 'mk':
                this.mokuai_id = s[1];
                break;
            case 'xk':
                this.xueke_id = s[1];
                break;
            case 'bb':
                this.last_ids = s[1];
                break;
            default:
                console.log('find no data')
                break;
        }
    }
}


class BindingModel extends BasicBind {

    static cls_catch = 'catch';
    /**
     * bd model with clicked data
     * @param {*} lx 
     * @param {*} mk 
     * @param {*} xk 
     * @param {*} last_ids 
     */
    constructor(menu, lx = -1, mk = -1, xk = -1, last_ids = []) {
        super(lx, mk, xk, last_ids);
        this.menu = menu;
        this.clk = lx;
    }

    get clkIndex() {
        if (this.clk.indexOf('_') == -1) {
            return -1;
        }
        let s = this.clk.split('_')[0];
        switch (s) {
            case 'lx':
                return 0;
            case 'mk':
                return 1;
            case 'xk':
                return 2
            default:
                return 3;

        }
    }

    static emptyDataLi() {
        return DomGen.createDom('li', { 'class': "emp" }, '[无数据]');
    }

    /**
     * 
     * @param {index of section} d
     * @returns string
     */
    getUppers = function (d) {
        if (d !== 0) {
            let arr = '',
                list = $("section");

            $.each(list,
                function (m, n) {
                    if (m < d) {
                        let obj = $(n).find("a.active");
                        if (obj.length > 0) {
                            arr += obj.attr("rel").split('_')[1] + ',';
                        }
                    }
                });

            return arr.substring(0, arr.length - 1);
        }
        return '';
    }

    /**
      * 
      * @param {class type: active/catch/null} type 
      * @returns this instance
      */
    getModel(type = 'active') {
        let sct = {}, secDom = $('section:lt(3)');
        switch (type) {
            case 'active':
                sct = secDom.find('a.active');
                break;
            case 'catch':
                sct = secDom.find('a.catch');
                break;
            default:
                sct = secDom.find('a.active,.catch');
                break;
        }

        for (let index = 0; index < 3; index++) {
            const element = $(sct[index]).attr("rel");
            if (element)
                this.genModel(element);
        }
        return this;
    }

    /**
     * gen model with selectedId
     * @param {*} str_xx 
     */
    genModel(str_xx) {
        let xx = str_xx.split('_'),
            _id = $('div.' + xx[0] + '>ul>li').eq(0)
                .find("a").attr("rel").split("_")[1];

        if (this.menu == '电子书列表') {
            return this.getActiveRel(4);
        } else {

            switch (xx[0]) {
                case 'lx':
                    this.leixing_id = xx[1] || _id;
                    break;
                case 'mk':
                    this.mokuai_id = xx[1] || _id;
                    break;
                case 'xk':
                    this.xueke_id = xx[1] || _id;
                    break;
            }
        }
    }

    /**
     * 
     * @param {*} d 
     * @param {*} isNo 
     * @returns 
     */
    getActiveRel(d, isNo = true) {

        if (d !== 0) {
            if (isNo && this.menu == '数据关联') {
                return this.getUppers(d);
            } else {

                let
                    list = $("section"),
                    bd = this;
                $.each(list,
                    function (m, n) {
                        if (m < d) {
                            let
                                obj = $(n).find('li.' + DataBindPage.cls_catch).find('a');
                            if (obj.length > 0) {

                                let title = $(obj).parents('div').attr("name"),
                                    _id = $(obj).attr('rel').split('_')[1];
                                switch (title) {
                                    case 'lx':
                                        bd.leixing_id = _id;
                                        break;
                                    case 'mk':
                                        bd.mokuai_id = _id;
                                        break;
                                    case 'xk':
                                        bd.xueke_id = _id;
                                        break;
                                    case 'bb':
                                        bd.last_ids[0] = _id;
                                }
                            } else {

                            }
                        }
                    });


                return bd;
            }

        }

    }


    hasVal() {
        return this.isValid();
    }
    isValid() {
        return this.mokuai_id !== -1 &&
            this.xueke_id !== -1 &&
            this.leixing_id !== -1 &&
            this.last_ids.length > 0;
    }
};
