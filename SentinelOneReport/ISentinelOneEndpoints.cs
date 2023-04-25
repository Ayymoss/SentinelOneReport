using RestEase;

namespace SentinelOneReport;

public interface ISentinelOneEndpoints
{
    [Header("Authorization")] string Authorization { get; set; }

    [Get(
        "/web/api/v2.1/agents?limit=1000&userActionsNeeded=reboot_needed,agent_suppressed_category,extended_exclusions_partially_accepted,incompatible_os,incompatible_os_category,missing_permissions_category,reboot_category,rebootless_without_dynamic_detection,unprotected,unprotected_category,upgrade_needed,user_action_needed,user_action_needed_bluetooth_per,user_action_needed_fda,user_action_needed_network,user_action_needed_rs_fda")]
    Task<HttpResponseMessage> GetActionRequiredComputers();
}
