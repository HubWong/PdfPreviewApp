class TableBase extends BaseReqModel {
    static clsSelected = 'cur'
    constructor(table_id, table_title = null) {
        super();
        this.$table = $('#' + table_id);
        this.table_title = table_title || this.$table.find('tfoot').text();
        this.operResult = false;
        this.popUpClass = OperResultDialog.ENV_Class.default;
        this.api_edit = '';
        this.api_del = '';
        this.api_add = '';
        this.api_delmany = '';
        this.api_query = '';
    }

    popDialog = (msg, isConfirm = false, cb = null) => {
        if (isConfirm) {
            OperResultDialog.confirm(msg, cb);
        } else {
            let dialog = new OperResultDialog(this.operResult, msg, this.popUpClass);
            dialog.popUp();
        }
    }
    get existed() {
        return this.$table != null;
    }

    get tableRows() {
        if (this.existed) {
            return this.$table.find('tbody').find('tr');
        }
        return null;
    }

    get tableHasRow() {
        if (this.existed) {
            return this.tableRows.length > 0;
        }
    }

    createEmpRow() {
        let td_len = this.$table.find('tr').find('td').length,
            tr = document.createElement('tr'),
            td = document.createElement('td');
        td.innerHTML = '没有数据';
        td.setAttribute('colspan', td_len);
        td.setAttribute('style', 'text-align:center');
        $(tr).append(td);
        this.$table.find('tbody').append(tr);
    }

    /**
     * add new data
     * @param {*} pageUrl 
     * @param {*} data 
     */
    rowAdd = (pageUrl, parms, sbmitUrl) => {
        this.request_view(pageUrl, parms, sbmitUrl)
    }

    rowEdit = (pageUrl, parms, sbmitUrl) => {
        this.request_view(pageUrl, parms, sbmitUrl)
    }

    rowsDel = () => {
        let slf = this,
            trs_del = this.$table.find('tbody [type=checkbox]:checked'),
            delFunc = () => {
                trs_del.forEach(r => {
                    slf.rowDel();
                });

                if (this.api_delmany) {
                    this.request_data(this.api_delmany, [], null, 'post')
                } else {
                    console.log('del many url is null')
                }
            };

        if (trs_del.length != 0) {
            this.popUpClass = OperResultDialog.ENV_Class.warning;
            this.popDialog('是否删除？', true, delFunc);
        }
        return trs_array; //not really del,just dom

    }
    rowDel = () => {
        ToolFunc.remove_ckd_tr('table', 'cur');
    }

    selectRow = (ele) => {
        $(ele).addClass(TableBase.clsSelected).siblings().removeClass(TableBase.clsSelected)
    }
}