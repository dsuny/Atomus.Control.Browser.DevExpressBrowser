using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraNavBar;
//using DevExpress.XtraScheduler.Localization;

namespace Atomus.Control.Browser
{
    public class ControlLocalizer
    {
        public static void InitLocalize(string IME = "kr")
        {
            switch (IME)
            {
                case "kr":
                    GridLocalizer.Active = new KoreanGridLocalizer();
                    //Localizer.Active = new KoreanEditorLocalizer();
                    //TreeListLocalizer.Active = new KoreanTreeListLocalizer();
                    //BarLocalizer.Active = new KoreanBarLocalizer();
                    //LayoutLocalizer.Active = new KoreanLayoutLocalizer();
                    //NavBarLocalizer.Active = new KoreanNavBarLocalizer();
                    //SchedulerLocalizer.Active = New KoreanSchedulerLocalizer();
                    break;
            }
        }

        ///<summary>
        ///KoreanGridLocalizer
        ///</summary>
        ///<remarks>KoreanGridLocalizer</remarks>
        public class KoreanGridLocalizer : GridLocalizer
        {
            public override string GetLocalizedString(GridStringId id)
            {
                switch (id)
                {
                    case GridStringId.CardViewNewCard: return "새로운 카드";
                    case GridStringId.CardViewQuickCustomizationButton: return "사용자정의";
                    case GridStringId.CardViewQuickCustomizationButtonFilter: return "필터";
                    case GridStringId.CardViewQuickCustomizationButtonSort: return "정렬:";
                    case GridStringId.ColumnViewExceptionMessage: return "값을 수정하시겠습니까?";
                    case GridStringId.CustomFilterDialog2FieldCheck: return "필드";
                    case GridStringId.CustomFilterDialogCancelButton: return "취소";
                    case GridStringId.CustomFilterDialogCaption: return "조건:";
                    case GridStringId.CustomFilterDialogClearFilter: return "초기화";
                    case GridStringId.CustomFilterDialogFormCaption: return "사용자 정의 필터";
                    case GridStringId.CustomFilterDialogOkButton: return "확인";
                    case GridStringId.CustomizationCaption: return "컬럼창";
                    case GridStringId.CustomizationColumns: return "컬럼";
                    case GridStringId.CustomizationFormColumnHint: return "컬럼을 가져오세요.";
                    case GridStringId.FilterBuilderApplyButton: return "적용";
                    case GridStringId.FilterBuilderCancelButton: return "취소";
                    case GridStringId.FilterBuilderCaption: return "필터 편집";
                    case GridStringId.FilterBuilderOkButton: return "확인";
                    case GridStringId.FilterPanelCustomizeButton: return "필터 편집";
                    case GridStringId.GridGroupPanelText: return "그룹하고자 하는 컬럼을 가져오세요.";
                    //case GridStringId.GridNewRowText: return "새로운 열";
                    case GridStringId.GridNewRowText: return "새로운 열을 입력하려면 클릭하세요.";
                    case GridStringId.MenuColumnBestFit: return "컬럼 최적화";
                    case GridStringId.MenuColumnFilterEditor: return "필터 편집";
                    case GridStringId.MenuColumnClearFilter: return "컬럼 최적화";
                    case GridStringId.MenuColumnClearSorting: return "필터 초기화";
                    case GridStringId.MenuColumnSortAscending: return "오름 순서 정렬";
                    case GridStringId.MenuColumnSortDescending: return "내림 순서 정렬";
                    case GridStringId.MenuColumnBestFitAllColumns: return "전체 컬럼 최적화";
                    //case GridStringId.MenuColumnClearFilter: return "컬럼 필터 초기화";
                    case GridStringId.MenuColumnUnGroup: return "컬럼 그룹 해제";
                    case GridStringId.MenuColumnColumnCustomization: return "컬럼 선택기";
                    case GridStringId.MenuColumnGroup: return "현재 컬럼 그룹하기";
                    case GridStringId.MenuColumnGroupBox: return "박스 사용 그룹하기";
                    case GridStringId.MenuGroupPanelClearGrouping: return "그룹 항목 초기화";
                    case GridStringId.MenuGroupPanelFullCollapse: return "전체 접기";
                    case GridStringId.MenuGroupPanelFullExpand: return "전체 펼치기";
                    case GridStringId.PopupFilterAll: return "(전체)";
                    case GridStringId.PopupFilterBlanks: return "(공백)";
                    case GridStringId.PopupFilterCustom: return "(사용자정의)";
                    case GridStringId.PopupFilterNonBlanks: return "(공백아님)";
                }

                return base.GetLocalizedString(id);
            }
        }
    }
}